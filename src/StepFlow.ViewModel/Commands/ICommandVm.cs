using StepFlow.Core.Commands;

namespace StepFlow.ViewModel.Commands
{
	public interface ICommandVm<out TTarget> : ITargetingCommand<TTarget>
		where TTarget : IWrapper
	{
	}
}
