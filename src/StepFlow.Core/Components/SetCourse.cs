namespace StepFlow.Core.Components
{
	public sealed class SetCourse : Subject
	{
		public Collided? Collided { get; set; }

		public Course Course { get; set; }
	}
}
