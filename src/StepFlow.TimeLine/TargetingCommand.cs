using System;

namespace StepFlow.TimeLine
{
	public abstract class TargetingCommand<TTarget> : ITargetingCommand<TTarget>
		where TTarget : class
	{
		protected TargetingCommand(TTarget target) => Target = target ?? throw new ArgumentNullException(nameof(target));

		public TTarget Target { get; }

		public abstract void Execute();
		public abstract void Revert();
	}
}
