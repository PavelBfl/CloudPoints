using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Common.Exceptions;
using StepFlow.View.Sketch;

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

		public static RectangleF Offset(this RectangleF rectangle, Thickness offset, SizeF size)
		{
			(float left, float right) = LinearOffset(rectangle.Left, rectangle.Right, offset.Left, offset.Right, size.Width);
			(float top, float bottom) = LinearOffset(rectangle.Top, rectangle.Bottom, offset.Top, offset.Bottom, size.Height);

			return RectangleF.FromLTRB(left, top, right, bottom);
		}

		private static (float min, float max) LinearOffset(float min, float max, Unit byMin, Unit byMax, float size)
		{
			float? resultMin = byMin.Kind switch
			{
				UnitKind.None => null,
				UnitKind.Pixels => min + byMin.Value,
				UnitKind.Ptc => Lerp(min, max, byMin.Value),
				_ => throw EnumNotSupportedException.Create(byMin.Kind),
			};

			float? resultMax = byMax.Kind switch
			{
				UnitKind.None => null,
				UnitKind.Pixels => max - byMax.Value,
				UnitKind.Ptc => Lerp(min, max, byMax.Value),
				_ => throw EnumNotSupportedException.Create(byMax.Kind),
			};

			if (resultMin is null)
			{
				if (resultMax is null)
				{
					var centerMin = min + ((max - min) - size) / 2;
					return (centerMin, centerMin + size);
				}
				else
				{
					return (resultMax.Value - size, resultMax.Value);
				}
			}
			else
			{
				if (resultMax is null)
				{
					return (resultMin.Value, resultMin.Value + size);
				}
				else
				{
					return (resultMin.Value, resultMax.Value);
				}
			}
		}

		private static float Lerp(float min, float max, float value) => (max - min) * value + min;
	}
}
