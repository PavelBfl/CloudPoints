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
			while (true)
			{
				var tickResult = SingleTick();
				switch (tickResult)
				{
					case TickResult.Execute:
						break;
					case TickResult.Wait: return false;
					case TickResult.Complete: return true;
					default: throw EnumNotSupportedException.Create(tickResult);
				}
			}
		}

		private TickResult SingleTick()
		{
			if (Current is null)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>?)Owner.CreateProxy(Scheduler);
				schedulerProxy?.Next();
				Current = schedulerProxy?.Current;
			}

			if (Current is { } current)
			{
				if (current.Duration == 0)
				{
					var executor = (IActionBaseProxy<ActionBase>?)Owner.CreateProxy(current.Executor);
					executor?.Execute();

					Current = null;
					return TickResult.Execute;
				}
				else
				{
					Current = current.Decrement();
					return TickResult.Wait;
				}
			}
			else
			{
				return TickResult.Complete;
			}
		}
	}
}
