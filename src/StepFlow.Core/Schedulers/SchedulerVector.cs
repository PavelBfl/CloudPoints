using System.Collections.Generic;
using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerVector : Scheduler
	{
		public Collided? Collided { get; set; }

		public ICollection<Vector2> Vectors { get; } = new List<Vector2>();

		public Vector2 CorrectVector { get; set; }

		public int IndexCourse { get; set; }
	}
}
