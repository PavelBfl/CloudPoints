using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using StepFlow.Common.Exceptions;

namespace StepFlow.Core
{
	public static class CourseExtensions
	{
		public static Point ToOffset(this Course course) => course switch
		{
			Course.Left => new Point(-1, 0),
			Course.LeftTop => new Point(-1, -1),
			Course.Top => new Point(0, -1),
			Course.RightTop => new Point(1, -1),
			Course.Right => new Point(1, 0),
			Course.RightBottom => new Point(1, 1),
			Course.Bottom => new Point(0, 1),
			Course.LeftBottom => new Point(-1, 1),
			_ => throw EnumNotSupportedException.Create(course),
		};

		public static Course Invert(this Course course) => course switch
		{
			Course.Left => Course.Right,
			Course.LeftTop => Course.RightBottom,
			Course.Top => Course.Bottom,
			Course.RightTop => Course.LeftBottom,
			Course.Right => Course.Left,
			Course.RightBottom => Course.LeftTop,
			Course.Bottom => Course.Top,
			Course.LeftBottom => Course.RightTop,
			_ => throw EnumNotSupportedException.Create(course),
		};

		public static IEnumerable<Point> AllLine(Point first, Point second)
		{
			if (first == second)
			{
				yield return first;
			}
			else if (Math.Abs(first.X - second.X) > Math.Abs(first.Y - second.Y))
			{
				foreach (var point in Line(first, second))
				{
					yield return point;
				}

				yield return second;
			}
			else
			{
				foreach (var point in Line(Reflect(first), Reflect(second)))
				{
					yield return Reflect(point);
				}

				yield return second;
			}
		}

		private static Point Reflect(Point point) => new Point(point.Y, point.X);

		private static IEnumerable<Point> Line(Point first, Point second)
		{
			var delta = new Point(
				Math.Abs(second.X - first.X),
				Math.Abs(second.Y - first.Y)
			);

			var error = 0;
			var deltaError = delta.Y;
			var y = first.Y;
			var step = Math.Sign(second.Y - first.Y);

			var result = new List<Point>();

			var stepX = Math.Sign(second.X - first.X);
			for (var iX = first.X; iX != second.X; iX += stepX)
			{
				result.Add(new Point(iX, y));
				error += deltaError;

				if (error >= delta.X)
				{
					y += step;
					error -= delta.X;
				}
			}
			return result;
		}
	}
}
