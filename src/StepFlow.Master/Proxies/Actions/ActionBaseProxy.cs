using StepFlow.Core.Actions;

namespace StepFlow.Master.Proxies.Actions
{
	public interface IActionBaseProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : ActionBase
	{
		void Execute();
	}

	internal abstract class ActionBaseProxy<TTarget> : ProxyBase<TTarget>, IActionBaseProxy<TTarget>
		where TTarget : ActionBase
	{
		protected ActionBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public abstract void Execute();
	}
}
