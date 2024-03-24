using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IElementBaseProxy<out TTarget> : IProxyBase<TTarget>
		where TTarget : ElementBase
	{
		void ChangeProperty(object sender, string propertyName);
	}

	internal class ElementBaseProxy<TTarget> : ProxyBase<TTarget>, IElementBaseProxy<TTarget>
		where TTarget : ElementBase
	{
		public ElementBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public virtual void ChangeProperty(object sender, string propertyName)
		{
		}
	}
}
