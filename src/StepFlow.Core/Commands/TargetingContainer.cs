using System.Collections.Generic;

namespace StepFlow.Core.Commands
{
	internal class TargetingContainer<T>
	{
		public TargetingContainer(T target) => Target = target;

		public T Target { get; }

		public IList<ITargetingCommand<T>> Commands { get; } = new List<ITargetingCommand<T>>();
	}
}
