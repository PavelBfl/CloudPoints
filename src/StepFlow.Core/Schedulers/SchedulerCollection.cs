using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerCollection : Scheduler
	{
		public int Index { get; set; }

		public IList<Turn> Turns { get; } = new List<Turn>();
	}
}
