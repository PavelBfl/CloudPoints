using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Intersection
{
	internal readonly struct ShapeRaw
	{
		private static Rectangle CreateBounds(Rectangle[] rectangles)
		{
			Rectangle? result = null;
			foreach (var rectangle in rectangles)
			{
				result = result.HasValue ? Rectangle.Union(result.GetValueOrDefault(), rectangle) : rectangle;
			}

			return result ?? Rectangle.Empty;
		}

		public ShapeRaw(IEnumerable<Rectangle> rectangles)
		{
			this.rectangles = rectangles.ToArray();
			Bounds = CreateBounds(this.rectangles);
		}

		public Rectangle Bounds { get; }

		private readonly Rectangle[]? rectangles;

		public Rectangle[] Rectangles => rectangles ?? Array.Empty<Rectangle>();
	}
}
