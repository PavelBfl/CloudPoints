using System;

namespace StepFlow.ViewModel
{
	[Flags]
	public enum NodeState
	{
		None = 0x0,
		Current = 0x1,
		Planned = 0x2,
	}
}
