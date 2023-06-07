using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	public interface IQueue<T> : IReadOnlyDictionary<long, IReadOnlyCollection<ITargetingCommand<T>>>
	{
		IReadOnlyCollection<ITargetingCommand<T>>? Dequeue(long key);

		ITargetingCommand<T>? Add(long time, IBuilder<T> builder);
	}
}
