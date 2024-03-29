﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Intersection
{
	public sealed class Context : ICollection<ShapeBase>
	{
		private List<List<Relation>> Relations { get; } = new List<List<Relation>>();

		private List<ShapeBase> Shapes { get; } = new List<ShapeBase>();

		public int Count => Shapes.Count;

		public bool IsReadOnly => false;

		internal void Reset(ShapeBase shape)
		{
			if (shape is null)
			{
				throw new ArgumentNullException(nameof(shape));
			}

			if (shape.Context != this)
			{
				throw new InvalidOperationException();
			}

			if (shape.Index < 0)
			{
				return;
			}

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

		public void Add(ShapeBase shape)
		{
			if (shape is null)
			{
				throw new ArgumentNullException(nameof(shape));
			}

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

		public bool Remove(ShapeBase shape)
		{
			if (shape is null)
			{
				throw new ArgumentNullException(nameof(shape));
			}

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

		public bool Contains(ShapeBase item) => Shapes.Contains(item);

		public void CopyTo(ShapeBase[] array, int arrayIndex) => Shapes.CopyTo(array, arrayIndex);

		public IEnumerator<ShapeBase> GetEnumerator() => Shapes.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
