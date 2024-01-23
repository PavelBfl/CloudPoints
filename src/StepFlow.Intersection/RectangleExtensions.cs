using System;
using System.Drawing;

namespace StepFlow.Intersection
{
	public static class RectangleExtensions
	{
		public static Point GetCenter(this Rectangle rectangle)
		{
			if (rectangle.IsEmpty)
			{
				throw new InvalidOperationException();
			}

			return new Point(
				rectangle.X + rectangle.Width / 2,
				rectangle.Y + rectangle.Height / 2
			);
		}

		public static Rectangle Create(Point center, int radius)
		{
			if (radius < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(radius));
			}

			var radiusWithoutCenter = radius - 1;
			var diameter = radiusWithoutCenter * 2 + 1;
			return new Rectangle(
				center.X - radiusWithoutCenter,
				center.Y - radiusWithoutCenter,
				diameter,
				diameter
			);
		}
	}
}
