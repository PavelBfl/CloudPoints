using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerRunnerProxy : IProxyBase<SchedulerRunner>
	{
		int Begin { get; set; }

		Turn? Current { get; set; }

		Scheduler? Scheduler { get; set; }

		void OnTick();
	}

	internal sealed class SchedulerRunnerProxy : ProxyBase<SchedulerRunner>, ISchedulerRunnerProxy
	{
		public SchedulerRunnerProxy(PlayMaster owner, SchedulerRunner target) : base(owner, target)
		{
		}

		public int Begin { get => Target.Begin; set => SetValue(x => x.Begin, value); }

		public Turn? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public Scheduler? Scheduler { get => Target.Scheduler; set => SetValue(x => x.Scheduler, value); }

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
				if (Owner.TimeAxis.Count == Begin + current.Duration)
				{
					var executor = (ITurnExecutor?)Owner.CreateProxy(current.Executor);
					executor?.Execute();

					Begin += (int)current.Duration;
					Current = null;
					return true;
				}
			}

			return false;
		}
	}
}
