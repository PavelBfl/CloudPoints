using System.Numerics;

namespace StepFlow.Core.Schedulers
{
	public sealed class CourseVector : Subject
	{
		public Vector2 Value { get; set; }

		public Matrix3x2 Delta { get; set; } = Matrix3x2.Identity;
	}
}
