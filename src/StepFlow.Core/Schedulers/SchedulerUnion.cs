using System.Collections.Generic;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerUnion : Scheduler
	{
		public int Index { get; set; }

		public IList<Scheduler> Schedulers { get; } = new List<Scheduler>();
	}
}
