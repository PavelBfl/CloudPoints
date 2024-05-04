using System.Numerics;

namespace StepFlow.Core.Schedulers
{
	public enum DeltaOperation
	{
		Sum,
		Mul,
	}

	public sealed class CourseVector : Subject
	{
		public Vector2 Value { get; set; }

		public DeltaOperation Operation { get; set; }

		public Vector2 Delta { get; set; }
	}
}
