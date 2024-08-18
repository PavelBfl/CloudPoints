using StepFlow.Common.Exceptions;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System;

namespace StepFlow.Common
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

		public static Course? ToCourse(this Point point) => point switch
		{
			{ X: -1, Y: 0 } => Course.Left,
			{ X: -1, Y: -1 } => Course.LeftTop,
			{ X: 0, Y: -1 } => Course.Top,
			{ X: 1, Y: -1 } => Course.RightTop,
			{ X: 1, Y: 0 } => Course.Right,
			{ X: 1, Y: 1 } => Course.RightBottom,
			{ X: 0, Y: 1 } => Course.Bottom,
			{ X: -1, Y: 1 } => Course.LeftBottom,
			_ => null,
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
	}
}
