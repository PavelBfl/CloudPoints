namespace StepFlow.Core.Commands
{
	public interface IResolver<in T>
	{
		bool CanExecute(T target);
	}
}
