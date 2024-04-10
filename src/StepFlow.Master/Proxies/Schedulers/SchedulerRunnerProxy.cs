using StepFlow.Core.Actions;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Actions;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerRunnerProxy : IProxyBase<SchedulerRunner>
	{
		Turn? Current { get; set; }

		Scheduler? Scheduler { get; set; }

		void OnTick();
	}

	internal sealed class SchedulerRunnerProxy : ProxyBase<SchedulerRunner>, ISchedulerRunnerProxy
	{
		public SchedulerRunnerProxy(PlayMaster owner, SchedulerRunner target) : base(owner, target)
		{
		}

		public Turn? Current { get => Target.Current; set => SetValue(value, nameof(Target.Current)); }

		public Scheduler? Scheduler { get => Target.Scheduler; set => SetValue(value); }

		public void OnTick()
		{
			while (SingleTick()) ;
		}

		private bool SingleTick()
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
					return true;
				}
				else
				{
					Current = current.Decrement();
				}
			}

			return false;
		}
	}
}
