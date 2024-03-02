using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerProxy<out TScheduler> : IElementBaseProxy<TScheduler>
		where TScheduler : Scheduler
	{
		Turn? Current { get; set; }

		void Next();
	}

	internal abstract class SchedulerProxy<TScheduler> : ElementBaseProxy<TScheduler>, ISchedulerProxy<TScheduler>
		where TScheduler : Scheduler
	{
		protected SchedulerProxy(PlayMaster owner, TScheduler target) : base(owner, target)
		{
		}

		public Turn? Current { get => Target.Current; set => SetValue(x => x.Current, value); }

		public abstract void Next();
	}
}
