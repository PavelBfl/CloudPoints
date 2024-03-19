using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerLimit : Scheduler
	{
		public Scheduler? Source { get; set; }

		public Scheduler GetSourceRequired() => PropertyRequired(Source, nameof(Source));

		public Scale? Range { get; set; }

		public Scale GetRangeRequired() => PropertyRequired(Range, nameof(Range));
	}
}
