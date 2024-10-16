using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	internal class Segment : IReadOnlyCollection<Shape>
	{
		public Segment(Context context, Segment? owner, Rectangle bounds)
		{
			NullValidate.ThrowIfArgumentNull(context, nameof(context));

			Context = context;
			Owner = owner;
			Bounds = bounds;
		}

		public Segment? Owner { get; }

		public Context Context { get; }

		public Rectangle Bounds { get; }

		public IList<Shape> Common { get; } = new List<Shape>();

		public virtual int Count => Common.Count;

		public virtual void RemoveAt(int index)
		{
			Common.RemoveAt(index);

			for (var i = index; i < Common.Count; i++)
			{
				Common[i].Index = i;
			}

			CommonReset();
		}

		public virtual void Add(Shape shape)
		{
			CommonReset();

			shape.SetOwner(this, Common.Count);
			Common.Add(shape);
		}

		public bool TryAdd(Shape shape)
		{
			if (Bounds.Contains(shape.Bounds))
			{
				Add(shape);
				return true;
			}
			else
			{
				return false;
			}
		}

		public IEnumerable<Shape>? TryGetCollision(Shape shape)
		{
			NullValidate.ThrowIfArgumentNull(shape, nameof(shape));

			if (Bounds.Contains(shape.Bounds))
			{
				return GetCollisions(shape);
			}
			else
			{
				return null;
			}
		}

		public void CommonReset()
		{
			foreach (var shape in Common)
			{
				shape.Reset();
			}
		}

		public virtual IEnumerable<Shape> GetCollisions(Shape other)
		{
			NullValidate.ThrowIfArgumentNull(other, nameof(other));

			foreach (var shape in this)
			{
				if (shape.IntersectsWith(other))
				{
					yield return shape;
				}
			}
		}

		public virtual IEnumerator<Shape> GetEnumerator() => Common.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
