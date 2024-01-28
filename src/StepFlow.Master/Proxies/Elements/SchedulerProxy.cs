using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface ISchedulerProxy : IElementBaseProxy<Scheduler>
	{
		new Subject? Target { get; set; }

		int Begin { get; set; }

		int CurrentIndex { get; set; }

		IList<Turn> Queue { get; }

		void OnTick();
	}

	internal sealed class SchedulerProxy : ElementBaseProxy<Scheduler>, ISchedulerProxy
	{
		public SchedulerProxy(PlayMaster owner, Scheduler target) : base(owner, target)
		{
		}

		public new Subject? Target { get => base.Target.Target; set => SetValue(x => x.Target, value); }

		public int Begin { get => base.Target.Begin; set => SetValue(x => x.Begin, value); }

		public int CurrentIndex { get => base.Target.CurrentIndex; set => SetValue(x => x.CurrentIndex, value); }

		public IList<Turn> Queue => CreateListProxy(base.Target.Queue);

		public void OnTick()
		{
			while (SingleTick()) ;
		}

		private bool SingleTick()
		{
			if (0 <= CurrentIndex && CurrentIndex < Queue.Count)
			{
				var currentTurn = Queue[CurrentIndex];

				if (Owner.TimeAxis.Count == (Begin + currentTurn.Duration))
				{
					var executor = (ITurnExecutor?)Owner.CreateProxy(currentTurn.Executor);
					executor?.Execute();

					Begin += (int)currentTurn.Duration;
					CurrentIndex++;
					return true;
				}
			}

			return false;
		}
	}
}
