using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScheduledProxy : IProxyBase<IScheduled>
	{
		long QueueBegin { get; set; }

		IList<Turn> Queue { get; }
	}

	internal sealed class ScheduledProxy : ProxyBase<IScheduled>, IScheduledProxy
	{
		public ScheduledProxy(PlayMaster owner, IScheduled target) : base(owner, target)
		{
		}

		public long QueueBegin { get => Target.QueueBegin; set => SetValue(x => x.QueueBegin, value); }

		public IList<Turn> Queue => new ListProxy<Turn, IList<Turn>>(Owner, Target.Queue);
	}
}
