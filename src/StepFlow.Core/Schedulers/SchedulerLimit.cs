using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerLimit : Scheduler
	{
		public Scheduler? Source { get; set; }

		public Scale? Range { get; set; }
	}
}
