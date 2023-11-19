using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScheduledProxy : IProxyBase<Scheduled>
	{
		long QueueBegin { get; set; }

		IList<ITurnProxy> Queue { get; }

		void Dequeue();

		void Enqueue(long duration, ITurnExecutor? turn = null);
	}

	internal sealed class ScheduledProxy : ProxyBase<Scheduled>, IScheduledProxy
	{
		public ScheduledProxy(PlayMaster owner, Scheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public IList<ITurnProxy> Queue => new ListItemsProxy<Turn, IList<Turn>, ITurnProxy>(Owner, Target.Queue);

		public void Enqueue(long duration, ITurnExecutor? turn = null)
		{
			if (duration < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(duration));
			}

			var turnProxy = (ITurnProxy)Owner.CreateProxy(new Turn(duration, turn?.Target));

			var queue = Queue;
			if (!queue.Any())
			{
				QueueBegin = Owner.Time;
			}

			queue.Add(turnProxy);
		}

		public void Dequeue()
		{
			while (TryDequeueSingle())
			{
			}
		}

		private bool TryDequeueSingle()
		{
			if (Queue.FirstOrDefault() is { } first && (QueueBegin + first.Duration) == Owner.Time)
			{
				Queue.RemoveAt(0);
				first.Executor?.Execute();
				QueueBegin = Owner.Time;

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
