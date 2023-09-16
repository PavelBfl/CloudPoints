using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class Scheduled : ComponentBase
	{
		public Scheduled(Playground owner) : base(owner)
		{
			QueueComplete = new Event(Owner);
		}

		public long QueueBegin { get; set; }

		public List<Turn> Queue { get; } = new List<Turn>();

		public ICollection<IComponentChild> QueueComplete { get; }
	}
}
