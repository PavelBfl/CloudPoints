using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

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

		public static Vector2 GetCenter(this RectangleF rectangle)
		{
			if (rectangle.IsEmpty)
			{
				throw new InvalidOperationException();
			}

			return new Vector2(
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

		public static RectangleF Create(Vector2 center, float radius)
		{
			if (radius < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(radius));
			}
			else if (radius == 0)
			{
				return RectangleF.Empty;
			}
			else
			{
				return new RectangleF(
					center.X - radius,
					center.Y - radius,
					radius * 2,
					radius * 2
				);
			}
		}

		public static IEnumerable<Rectangle> Offset(this IEnumerable<Rectangle> rectangles, Point value)
		{
			foreach (var rectangle in rectangles)
			{
				rectangle.Offset(value);
				yield return rectangle;
			}
		}

		public static IEnumerable<RectangleF> Offset(this IEnumerable<RectangleF> rectangles, Vector2 value)
		{
			foreach (var rectangle in rectangles)
			{
				rectangle.Offset(value.X, value.Y);
				yield return rectangle;
			}
		}

		public static IEnumerable<Rectangle> Scale(this IEnumerable<Rectangle> rectangles, Point scale)
		{
			if (rectangles is null)
			{
				throw new ArgumentNullException(nameof(rectangles));
			}

			if (scale.X < 0 || scale.Y < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(scale));
			}

			foreach (var rectangle in rectangles)
			{
				yield return new Rectangle(
					rectangle.X * scale.X,
					rectangle.Y * scale.Y,
					rectangle.Width * scale.X,
					rectangle.Height * scale.Y
				);
			}
		}
	}
}
