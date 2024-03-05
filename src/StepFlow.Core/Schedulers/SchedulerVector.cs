using System.Collections.Generic;
using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerVector : Scheduler
	{
		public Collided? Collided { get; set; }

		public ICollection<CourseVector> Vectors { get; } = new List<CourseVector>();

		public Vector2 CorrectVector { get; set; }

		public int IndexCourse { get; set; }
	}
}
