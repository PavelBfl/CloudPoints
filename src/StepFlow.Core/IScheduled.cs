using StepFlow.Core.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public interface IScheduled
	{
		Axis<ITargetingCommand<object>> AxisTime { get; }

		long Time { get; }

		object? Buffer { get; set; }
	}

	public interface IScheduled<T> : IScheduled
	{
		IScheduler<T> Scheduler { get; }
	}
}
