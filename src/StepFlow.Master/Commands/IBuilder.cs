namespace StepFlow.Core.Commands
{
	public interface IBuilder<T>
	{
		ITargetingCommand<T> Build(T target);

		bool CanBuild(T target);
	}
}
