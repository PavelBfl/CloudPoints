using System;

namespace StepFlow.Core.Commands
{
	public abstract class Command<TTarget> : ITargetingCommand<TTarget>
	{
		public Command(TTarget target, IResolver<TTarget> resolver)
		{
			Target = target;
			Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
		}

		public abstract void Execute();

		public abstract void Revert();

		public bool CanExecute() => Resolver.CanExecute(Target);

		public TTarget Target { get; }

		public IResolver<TTarget> Resolver { get; }
	}
}
