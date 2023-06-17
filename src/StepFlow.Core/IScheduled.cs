using StepFlow.Core.Commands;

namespace StepFlow.Core
{
	public interface IScheduled<T>
	{
		IScheduler<T> Scheduler { get; }
	}
}
