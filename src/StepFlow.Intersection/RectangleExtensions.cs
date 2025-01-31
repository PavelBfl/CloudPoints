﻿using System;
using System.Collections.Generic;
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

		public static IEnumerable<Rectangle> Offset(this IEnumerable<Rectangle> rectangles, Point value)
		{
			foreach (var rectangle in rectangles)
			{
				rectangle.Offset(value);
				yield return rectangle;
			}
		}

		public static void OffsetWith(this IList<Rectangle> rectangles, Point value)
		{
			for (var i = 0; i < rectangles.Count; i++)
			{
				var rectangle = rectangles[i];
				rectangle.Offset(value);
				rectangles[i] = rectangle;
			}
		}

		public static bool IntersectWith(this IEnumerable<Rectangle> source, IEnumerable<Rectangle> other)
		{
			foreach (var sourceRectangle in source)
			{
				foreach (var otherRectangle in other)
				{
					if (sourceRectangle.IntersectsWith(otherRectangle))
					{
						return true;
					}
				}
			}

			return false;
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
