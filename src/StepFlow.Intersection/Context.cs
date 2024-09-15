using System.Collections.Generic;
using System.Drawing;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	public sealed class Context
	{
		public Context(Rectangle bounds)
		{
			Main = new SegmentCells(this, null, bounds);
		}

		private Segment Main { get; }

		public void Add(Shape shape)
		{
			NullValidate.ThrowIfArgumentNull(shape, nameof(shape));

			if (shape.Owner is { })
			{
				throw ExceptionBuilder.CreateShapeAlreadyContext();
			}

			Main.Add(shape);
		}

		public IEnumerable<Shape> GetCollisions(Shape shape)
		{
			NullValidate.ThrowIfArgumentNull(shape, nameof(shape));

			return Main.GetCollisions(shape);
		}
	}
}
