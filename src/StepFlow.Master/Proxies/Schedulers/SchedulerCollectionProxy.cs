using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerCollectionProxy : ISchedulerProxy<SchedulerCollection>
	{
		int Index { get; set; }
	}

	internal sealed class SchedulerCollectionProxy : SchedulerProxy<SchedulerCollection>, ISchedulerCollectionProxy
	{
		public SchedulerCollectionProxy(PlayMaster owner, SchedulerCollection target) : base(owner, target)
		{
		}

		public int Index { get => Target.Index; set => SetValue(x => x.Index, value); }

		public override void Next()
		{
			if (0 <= Index && Index < Target.Turns.Count)
			{
				Current = Target.Turns[Index];
				Index++;
			}
			else
			{
				Current = null;
			}
		}
	}
}
