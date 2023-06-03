using StepFlow.TimeLine;

namespace StepFlow.Core.Commands
{
	public interface ITargetingCommand<out TTarget> : ICommand
	{
		TTarget Target { get; }
	}
}
