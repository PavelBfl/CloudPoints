using System.Drawing;
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
	}
}
