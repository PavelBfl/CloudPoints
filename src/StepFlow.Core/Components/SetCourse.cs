namespace StepFlow.Core.Components
{
	public sealed class SetCourse : Subject
	{
		public SetCourse(Context owner) : base(owner)
		{
		}

		public ICollided? Collided { get; set; }

		public Course Course { get; set; }
	}
}
