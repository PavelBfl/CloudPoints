using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Common.Exceptions;

namespace StepFlow.Core
{
	public static class CourseExtensions
	{
		public const int FLAT_FACTOR = 5;
		public const int DIAGONAL_FACTOR = 7;

		public static int GetFactor(this Course course) => course switch
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

		public static Point ToPoint(this Course course) => course switch
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

		public static Vector2 ToVector(this Course course)
		{
			var point = course.ToPoint();
			return new Vector2(point.X, point.Y);
		}

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
					yield return GetCourse(prev.Value, point) ?? throw new InvalidOperationException();
				}

				prev = point;
			}
		}

		private static Course? GetCourse(Point current, Point next)
		{
			if (current.Y < next.Y)
			{
				if (current.X < next.X)
				{
					return Course.RightBottom;
				}
				else if (current.X > next.X)
				{
					return Course.LeftBottom;
				}
				else
				{
					return Course.Bottom;
				}
			}
			else if (current.Y > next.Y)
			{
				if (current.X < next.X)
				{
					return Course.RightTop;
				}
				else if (current.X > next.X)
				{
					return Course.LeftTop;
				}
				else
				{
					return Course.Top;
				}
			}
			else
			{
				if (current.X < next.X)
				{
					return Course.Right;
				}
				else if (current.X > next.X)
				{
					return Course.Left;
				}
				else
				{
					return null;
				}
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

		public static Course? GetCourseStep(Vector2 vector, int step)
		{
			if (vector == Vector2.Zero)
			{
				return null;
			}
			else if (MathF.Abs(vector.X) > MathF.Abs(vector.Y))
			{
				var pair = GetStep(vector, step);
				return GetCourse(pair.Current, pair.Next);
			}
			else
			{
				vector = new Vector2(vector.Y, vector.X);
				var pair = GetStep(vector, step);
				return GetCourse(Reflect(pair.Current), Reflect(pair.Next));
			}
		}

		private static (Point Current, Point Next) GetStep(Vector2 vector, int step)
		{
			var factor = vector.Y / vector.X;

			return (GetPointStep(factor, step), GetPointStep(factor, step + MathF.Sign(vector.X)));
		}

		private static Point GetPointStep(float factor, int step) => new Point(step, (int)MathF.Floor(step * factor));
	}
}
