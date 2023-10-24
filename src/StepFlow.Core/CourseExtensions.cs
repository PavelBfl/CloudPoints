using System;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Common.Exceptions;

namespace StepFlow.Core
{
	public static class CourseExtensions
	{
		public const long FLAT_FACTOR = 5;
		public const long DIAGONAL_FACTOR = 7;

		public static long GetFactor(this Course course) => course switch
		{
			Course.Right => FLAT_FACTOR,
			Course.Left => FLAT_FACTOR,
			Course.Top => FLAT_FACTOR,
			Course.Bottom => FLAT_FACTOR,
			Course.RightTop => DIAGONAL_FACTOR,
			Course.RightBottom => DIAGONAL_FACTOR,
			Course.LeftTop => DIAGONAL_FACTOR,
			Course.LeftBottom => DIAGONAL_FACTOR,
			_ => throw EnumNotSupportedException.Create(course),
		};

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

		public static IEnumerable<Course> GetPath(Point begin, Point end)
		{
			Point? prev = null;
			foreach (var point in AllLine(begin, end))
			{
				if (prev is { })
				{
					if (prev.Value.Y < point.Y)
					{
						if (prev.Value.X < point.X)
						{
							yield return Course.RightBottom;
						}
						else if (prev.Value.X > point.X)
						{
							yield return Course.LeftBottom;
						}
						else
						{
							yield return Course.Bottom;
						}
					}
					else if (prev.Value.Y > point.Y)
					{
						if (prev.Value.X < point.X)
						{
							yield return Course.RightTop;
						}
						else if (prev.Value.X > point.X)
						{
							yield return Course.LeftTop;
						}
						else
						{
							yield return Course.Top;
						}
					}
					else
					{
						if (prev.Value.X < point.X)
						{
							yield return Course.Right;
						}
						else if (prev.Value.X > point.X)
						{
							yield return Course.Left;
						}
						else
						{
							throw new InvalidOperationException();
						}
					}
				}

				prev = point;
			}
		}

		public static IEnumerable<Point> AllLine(Point begin, Point end)
		{
			if (begin == end)
			{
				yield return begin;
			}
			else if (Math.Abs(begin.X - end.X) > Math.Abs(begin.Y - end.Y))
			{
				foreach (var point in Line(begin, end))
				{
					yield return point;
				}

				yield return end;
			}
			else
			{
				foreach (var point in Line(Reflect(begin), Reflect(end)))
				{
					yield return Reflect(point);
				}

				yield return end;
			}
		}

		private static Point Reflect(Point point) => new Point(point.Y, point.X);

		private static IEnumerable<Point> Line(Point begin, Point end)
		{
			var delta = new Point(
				Math.Abs(end.X - begin.X),
				Math.Abs(end.Y - begin.Y)
			);

			var error = 0;
			var deltaError = delta.Y;
			var y = begin.Y;
			var step = Math.Sign(end.Y - begin.Y);

			var stepX = Math.Sign(end.X - begin.X);
			for (var iX = begin.X; iX != end.X; iX += stepX)
			{
				yield return new Point(iX, y);
				error += deltaError;

				if (error >= delta.X)
				{
					y += step;
					error -= delta.X;
				}
			}
		}
	}
}
