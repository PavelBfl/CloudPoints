using System;

namespace StepFlow.Core.Commands
{
	internal class TargetingBuilder<T> : ITargetingBuilder<T>
	{
		public TargetingBuilder(TargetingContainer<T> container, IBuilder<T> builder)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			Builder = builder ?? throw new ArgumentNullException(nameof(builder));
		}

		public T Target => Container.Target;

		private TargetingContainer<T> Container { get; }

		private IBuilder<T> Builder { get; }

		public ITargetingCommand<T> Build()
		{
			var result = Builder.Build(Target);
			Container.Commands.Add(result);
			return result;
		}

		public bool CanBuild() => Builder.CanBuild(Target);
	}
}
