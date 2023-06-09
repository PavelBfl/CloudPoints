using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	public interface IQueue<T> : IReadOnlyList<ITargetingCommand<T>>
	{
		IReadOnlyCollection<ITargetingCommand<T>>? Dequeue();

		ITargetingCommand<T>? Add(IBuilder<T> builder);
	}
}
