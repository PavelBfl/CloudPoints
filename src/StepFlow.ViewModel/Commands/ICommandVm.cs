using StepFlow.TimeLine;

namespace StepFlow.ViewModel.Commands
{
	public interface ICommandVm<out T> : IWrapper<T>
		where T : notnull, ICommand
	{
		
	}
}
