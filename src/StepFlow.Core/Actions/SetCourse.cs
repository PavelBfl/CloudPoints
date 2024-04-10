using StepFlow.Core.Components;

namespace StepFlow.Core.Actions
{
	public sealed class SetCourse : ActionBase
	{
		public Collided? Collided { get; set; }

		public Course Course { get; set; }
	}
}
