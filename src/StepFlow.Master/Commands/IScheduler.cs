namespace StepFlow.Core.Commands
{
	public interface IScheduler<T>
	{
		IBuildersCollection<T> Builders { get; }

		IQueue<T> Queue { get; }
	}
}
