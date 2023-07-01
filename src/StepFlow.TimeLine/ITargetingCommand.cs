namespace StepFlow.TimeLine
{
	public interface ITargetingCommand<out TTarget> : ICommand
	{
		TTarget Target { get; }
	}
}
