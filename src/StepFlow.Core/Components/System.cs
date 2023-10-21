using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class System : ComponentBase
	{
		public System(Playground owner) : base(owner)
		{
			OnFrame = new Event(Owner);
		}

		public ICollection<Handler> OnFrame { get; }
	}
}
