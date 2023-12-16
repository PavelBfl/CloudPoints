using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IElementBaseProxy<TTarget> : IProxyBase<TTarget>
		where TTarget : ElementBase
	{

	}

	internal class ElementBaseProxy<TTarget> : ProxyBase<TTarget>, IElementBaseProxy<TTarget>
		where TTarget : ElementBase
	{
		public ElementBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}
	}
}
