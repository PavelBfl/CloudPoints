using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public interface IScheduled
	{
		long QueueBegin { get; set; }

		IList<Turn> Queue { get; }
	}
}
