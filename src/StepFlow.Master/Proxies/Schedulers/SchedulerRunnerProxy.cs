using StepFlow.Common.Exceptions;
using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Actions;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerRunnerProxy : IProxyBase<SchedulerRunner>
	{
		Turn? Current { get; set; }

		Scheduler? Scheduler { get; set; }

		bool OnTick();
	}

	internal sealed class SchedulerRunnerProxy : ProxyBase<SchedulerRunner>, ISchedulerRunnerProxy
	{
		private enum TickResult
		{
			Execute,
			Wait,
			Complete,
		}

		public SchedulerRunnerProxy(PlayMaster owner, SchedulerRunner target) : base(owner, target)
		{
		}

		public Turn? Current { get => Target.Current; set => SetValue(value, nameof(Target.Current)); }

		public Scheduler? Scheduler { get => Target.Scheduler; set => SetValue(value); }

		public bool OnTick()
		{
			var current = Current;
			while (true)
			{
				SingleTick(out var tickResult, ref current);
				switch (tickResult)
				{
					case TickResult.Execute:
						break;
					case TickResult.Wait:
						Current = current;
						return false;
					case TickResult.Complete:
						Current = current;
						return true;
					default: throw EnumNotSupportedException.Create(tickResult);
				}
			}
		}

		private void SingleTick(out TickResult tickResult, ref Turn? current)
		{
			if (current is null)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>?)Owner.CreateProxy(Scheduler);
				schedulerProxy?.Next();
				current = schedulerProxy?.Current;
			}

			if (current is { } currentInstance)
			{
				if (currentInstance.Duration == 0)
				{
					var executor = (IActionBaseProxy<ActionBase>?)Owner.CreateProxy(currentInstance.Executor);
					executor?.Execute();

					tickResult = TickResult.Execute;
					current = null;
				}
				else
				{
					tickResult = TickResult.Wait;
					current = currentInstance.Decrement();
				}
			}
			else
			{
				tickResult = TickResult.Complete;
				current = null;
			}
		}
	}
}
