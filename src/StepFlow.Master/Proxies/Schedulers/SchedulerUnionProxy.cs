using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;

namespace StepFlow.Master.Proxies.Schedulers
{
	public interface ISchedulerUnionProxy : ISchedulerProxy<SchedulerUnion>
	{
		int Index { get; set; }
	}

	internal sealed class SchedulerUnionProxy : SchedulerProxy<SchedulerUnion>, ISchedulerUnionProxy
	{
		public SchedulerUnionProxy(PlayMaster owner, SchedulerUnion target) : base(owner, target)
		{
		}

		public int Index { get => Target.Index; set => SetValue(x => x.Index, value); }

		public override void Next()
		{
			while (true)
			{
				if (TryNext(out var result))
				{
					Current = result;
					return;
				}
			}
		}

		private bool TryNext(out Turn? result)
		{
			if (0 <= Index && Index < Target.Schedulers.Count)
			{
				var currentScheduler = Target.Schedulers[Index];
				var currentSchedulerProxy = (ISchedulerProxy<Scheduler>)Owner.CreateProxy(currentScheduler);

				currentSchedulerProxy.Next();
				if (currentSchedulerProxy.Current is { } currentTurn)
				{
					result = currentTurn;
					return true;
				}
				else
				{
					Index++;
					result = null;
					return false;
				}
			}
			else
			{
				result = null;
				return true;
			}
		}
	}
}
