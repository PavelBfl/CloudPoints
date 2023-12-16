using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentBaseProxy<TTarget> : IProxyBase<TTarget>
		where TTarget : ComponentBase
	{
		IElementBaseProxy<ElementBase>? Element { get; set; }
	}

	internal class ComponentBaseProxy<TTarget> : ProxyBase<TTarget>, IComponentBaseProxy<TTarget>
		where TTarget : ComponentBase
	{
		public ComponentBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public IElementBaseProxy<ElementBase>? Element
		{
			get => (IElementBaseProxy<ElementBase>?)Owner.CreateProxy(Target.Element);
			set => SetValue(x => x.Element, value?.Target);
		}
	}
}
