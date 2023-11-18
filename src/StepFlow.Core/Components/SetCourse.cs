namespace StepFlow.Core.Components
{
	public sealed class SetCourse : Subject
	{
		public ICollided? Collided { get; set; }

		public Course Course { get; set; }
	}
}
