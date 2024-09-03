using System;
using System.Collections;
using System.Collections.Generic;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	public sealed class Context : ICollection<Shape>
	{
		private void ValidateShape(Shape? shape, bool expectEnable)
		{
			NullValidate.ThrowIfArgumentNull(shape, nameof(shape));

			if (shape.Context != this)
			{
				throw ExceptionBuilder.CreateUnknownContext();
			}

			if (shape.IsEnable != expectEnable)
			{
				throw ExceptionBuilder.CreateUnexpectedEnable();
			}
		}

		private List<List<Relation>> Relations { get; } = new List<List<Relation>>();

		private List<Shape> Shapes { get; } = new List<Shape>();

		public int Count => Shapes.Count;

		public bool IsReadOnly => false;

		internal void Reset(Shape shape)
		{
			ValidateShape(shape, true);

			if (shape.Index > 0)
			{
				foreach (var row in Relations[shape.Index - 1])
				{
					row.Reset();
				}
			}

			foreach (var row in Relations)
			{
				if (shape.Index < row.Count)
				{
					row[shape.Index].Reset();
				}
			}

			Collisions = null;
		}

		public void Add(Shape shape)
		{
			ValidateShape(shape, false);

			shape.Index = Shapes.Count;
			Shapes.Add(shape);

			if (Shapes.Count > 1)
			{
				var row = new List<Relation>();
				for (var i = 0; i < Shapes.Count - 1; i++)
				{
					row.Add(new Relation(shape, Shapes[i]));
				}
				Relations.Add(row);
			}

			Collisions = null;
		}

		public bool Remove(Shape shape)
		{
			ValidateShape(shape, true);

			if (shape.Index < 0)
			{
				throw new InvalidOperationException();
			}

			if (shape.Index > 0)
			{
				Relations.RemoveAt(shape.Index - 1);
			}
			else if (Relations.Count > 0)
			{
				// После отчистки эта строка будет хранить пустой список, поэтому удаляем сразу всю строку
				Relations.RemoveAt(0);
			}
			foreach (var row in Relations)
			{
				if (shape.Index < row.Count)
				{
					row.RemoveAt(shape.Index);
				}
			}

			Shapes.RemoveAt(shape.Index);
			for (var i = shape.Index; i < Shapes.Count; i++)
			{
				Shapes[i].Index = i;
			}

			shape.Index = -1;

			Collisions = null;
			return true;
		}

		private IReadOnlyList<Relation>? Collisions { get; set; }

		public IReadOnlyList<Relation> GetCollisions()
		{
			if (Collisions is null)
			{
				var result = new List<Relation>();

				for (var i = 0; i < Relations.Count; i++)
				{
					var row = Relations[i];

					for (var j = 0; j < row.Count; j++)
					{
						var relation = row[j];

						if (relation.IsCollision)
						{
							result.Add(relation);
						}
					}
				}

				Collisions = result;
			}

			return Collisions;
		}

		public void Clear()
		{
			Relations.Clear();
			Shapes.Clear();
		}

		public bool Contains(Shape item) => Shapes.Contains(item);

		public void CopyTo(Shape[] array, int arrayIndex) => Shapes.CopyTo(array, arrayIndex);

		public IEnumerator<Shape> GetEnumerator() => Shapes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public CollisionInfo GetCollisionInfo(Shape shape)
		{
			ValidateShape(shape, true);

			return new CollisionInfo(this, shape);
		}

		public sealed class CollisionInfo
		{
			public CollisionInfo(Context context, Shape shape)
			{
				NullValidate.ThrowIfArgumentNull(context, nameof(context));
				NullValidate.ThrowIfArgumentNull(shape, nameof(shape));

				Context = context;
				Current = shape;
			}

			public Context Context { get; }

			public Shape Current { get; }

			public IEnumerable<Shape> GetCollisions()
			{
				for (var i = 0; i < Context.Shapes.Count; i++)
				{
					if (i < Current.Index)
					{
						var relations = Context.Relations[Current.Index - 1];
						var relation = relations[i];
						if (relation.IsCollision)
						{
							yield return relation.Left == Current ? relation.Right : relation.Left;
						}
					}
					else if (i > Current.Index)
					{
						var relations = Context.Relations[i - 1];
						var relation = relations[Current.Index];
						if (relation.IsCollision)
						{
							yield return relation.Left == Current ? relation.Right : relation.Left;
						}
					}
				}
			}
		}
	}
}
