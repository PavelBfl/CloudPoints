namespace StepFlow.Core.Commands
{
	public interface ITargetingBuilder<out T>
	{
		T Target { get; }

		ITargetingCommand<T> Build(long key);

		bool CanBuild();
	}
}
