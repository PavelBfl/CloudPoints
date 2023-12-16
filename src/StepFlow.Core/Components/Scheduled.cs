using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public interface IScheduled
	{
		long QueueBegin { get; set; }

		IList<Turn> Queue { get; }
	}

	public sealed class Scheduled : ComponentBase, IScheduled
	{
		public long QueueBegin { get; set; }

		public IList<Turn> Queue { get; } = new List<Turn>();
	}
}
