using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerVector : Scheduler
	{
		public Collided? Collided { get; set; }

		public Collided GetCollidedRequired() => PropertyRequired(Collided, nameof(Collided));

		public ICollection<CourseVector> Vectors { get; } = new List<CourseVector>();
	}
}
