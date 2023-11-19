using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public interface IScheduled
	{
		long QueueBegin { get; set; }

		IList<Turn> Queue { get; }
	}

	public sealed class Scheduled : Subject, IScheduled
	{
		public long QueueBegin { get; set; }

		public IList<Turn> Queue { get; } = new List<Turn>();
	}
}
