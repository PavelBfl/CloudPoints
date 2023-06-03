namespace StepFlow.Core.Commands
{
	public abstract class Command<TTarget> : ITargetingCommand<TTarget>
	{
		public Command(TTarget target) => Target = target;

		public abstract void Execute();

		public abstract void Revert();

		public TTarget Target { get; }
	}
}
