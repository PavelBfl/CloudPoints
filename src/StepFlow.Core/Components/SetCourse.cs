namespace StepFlow.Core.Components
{
	public sealed class SetCourse : Handler
	{
		public SetCourse(Playground owner) : base(owner)
		{
		}

		public Course Course { get; set; }
	}
}
