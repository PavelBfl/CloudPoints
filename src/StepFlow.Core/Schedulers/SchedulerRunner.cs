using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerRunner : Subject
	{
		public int Begin { get; set; }

		public Turn? Current { get; set; }

		public Scheduler? Scheduler { get; set; }
	}
}
