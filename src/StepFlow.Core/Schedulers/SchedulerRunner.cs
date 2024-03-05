using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerRunner : Subject
	{
		public Turn? Current { get; set; }

		public Scheduler? Scheduler { get; set; }
	}
}
