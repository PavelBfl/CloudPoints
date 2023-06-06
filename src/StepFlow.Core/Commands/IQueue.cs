using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	public interface IQueue<T> : IReadOnlyList<ITargetingCommand<T>>
	{
		ITargetingCommand<T>? Dequeue();
	}
}
