namespace StepFlow.Core.Components
{
	public sealed class SetCourse : ComponentBase
	{
		public SetCourse(Playground owner) : base(owner)
		{
		}

		public Course Course { get; set; }
	}
}
