using System;

namespace StepFlow.Core.Commands
{
	internal class TargetingBuilder<T> : ITargetingBuilder<T>
	{
		public TargetingBuilder(ITargetingContainer<T> container, IBuilder<T> builder)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			Builder = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public T Target => Container.Target;

		private ITargetingContainer<T> Container { get; }

		private IBuilder<T> Builder { get; }

		public ITargetingCommand<T> Build(long key)
		{
			var result = Builder.Build(Target);
			Container.Queue.Add(key, result);
			return result;
		}

		public bool CanBuild() => Builder.CanBuild(Target);
	}
}
