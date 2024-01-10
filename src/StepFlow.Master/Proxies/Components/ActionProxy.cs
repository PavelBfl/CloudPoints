using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IActionProxy : IProxyBase<Action>
	{
		long Begin { get; set; }

		long Duration { get; set; }

		Subject? Executor { get; set; }
	}

	internal class ActionProxy : ProxyBase<Action>, IActionProxy
	{
		public ActionProxy(PlayMaster owner, Action target) : base(owner, target)
		{
		}

		public long Begin { get => Target.Begin; set => SetValue(x => x.Begin, value); }

		public long Duration { get => Target.Duration; set => SetValue(x => x.Duration, value); }

		public Subject? Executor { get => Target.Executor; set => SetValue(x => x.Executor, value); }
	}
}
