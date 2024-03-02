using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;

namespace StepFlow.Core.Elements
{
	public class Material : ElementBase
	{
		private Scale? strength;

		public Scale? Strength { get => strength; set => SetComponent(ref strength, value); }

		private Collided? body;

		public Collided? Body { get => body; set => SetComponent(ref body, value); }

		public Action? CurrentAction { get; set; }

		public int Speed { get; set; }

		public ICollection<SchedulerRunner> Schedulers { get; } = new HashSet<SchedulerRunner>();
	}
}
