using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public static class IntersectionContextExtensions
	{
		public static RefCounter<ShapeCell> CreateCell(this Context context, Rectangle bounds)
			=> new RefCounter<ShapeCell>(context, new ShapeCell(bounds));

		public static RefCounter<ShapeContainer> CreateContainer(this Context context, IEnumerable<Rectangle> bounds)
			=> new RefCounter<ShapeContainer>(context, new ShapeContainer(bounds));
	}
}