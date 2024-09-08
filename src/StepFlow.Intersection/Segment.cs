using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
			CommonReset();
		}

		public virtual Shape Add(ShapeRaw shapeRaw)
		{
			CommonReset();

			var result = new Shape(this, Common.Count, shapeRaw);
			Common.Add(result);
			return result;
		}

		public bool TryAdd(ShapeRaw shapeRaw, [MaybeNullWhen(false)] out Shape shape)
		{
			if (Bounds.Contains(shapeRaw.Bounds))
			{
				shape = Add(shapeRaw);
				return true;
			}
			else
			{
				shape = null;
				return false;
			}
		}

		public void CommonReset()
		{
			foreach (var shape in Common)
			{
				shape.Reset();
			}
		}

		public virtual IEnumerator<Shape> GetEnumerator() => Common.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
