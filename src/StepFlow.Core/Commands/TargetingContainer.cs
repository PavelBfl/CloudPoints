namespace StepFlow.Core.Commands
{
	internal interface ITargetingContainer<T>
	{
		T Target { get; }

		Queue<T> Queue { get; }
	}
}
