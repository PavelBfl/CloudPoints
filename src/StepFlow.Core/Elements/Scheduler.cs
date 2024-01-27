using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Scheduler : ElementBase
	{
		public Subject? Target { get; set; }

		public Scale? Cooldown { get; set; }

		public int? RepeatCount { get; set; }
	}

	public class PathScheduler : Scheduler
	{
		public int CurrentPathIndex { get; set; }

		public IList<Course> Path { get; } = new List<Course>();
	}
}
