namespace StepFlow.Core.Components
{
	public sealed class SetCourse : ComponentBase
	{
		public Collided? Collided { get; set; }

		public Course Course { get; set; }
	}
}
