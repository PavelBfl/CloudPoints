using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public sealed class Context
	{
		public Shape CreateShape(IEnumerable<Rectangle> rectangles) => Main.Add(new ShapeRaw(rectangles));

		public Context(Rectangle bounds)
		{
			Main = new SegmentCells(this, null, bounds);
		}

		private Segment Main { get; }
	}
}
