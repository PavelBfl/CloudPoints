using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerLimitProxy : ISchedulerProxy<SchedulerLimit>
	{

	}

	internal sealed class SchedulerLimitProxy : SchedulerProxy<SchedulerLimit>, ISchedulerLimitProxy
	{
		public SchedulerLimitProxy(PlayMaster owner, SchedulerLimit target) : base(owner, target)
		{
		}

		public Scheduler Source { get => Target.GetSourceRequired(); set => SetValue(Subject.PropertyRequired(value, nameof(Target.Source))); }

		public Scale Range { get => Target.GetRangeRequired(); set => SetValue(Subject.PropertyRequired(value, nameof(Target.Range))); }

		public override void Next()
		{
			if (Range.Value < Range.Max)
			{
				var schedulerProxy = (ISchedulerProxy<Scheduler>)Owner.CreateProxy(Source);
				schedulerProxy.Next();
				Current = schedulerProxy.Current;

				if (Current is { } current)
				{
					var rangeProxy = (ScaleProxy)Owner.CreateProxy(Range);
					rangeProxy.Add((int)current.Duration);
				}
			}
			else
			{
				Current = null;
			}
		}
	}
}
