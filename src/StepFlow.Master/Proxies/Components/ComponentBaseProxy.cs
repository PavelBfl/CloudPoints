using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentBaseProxy<TTarget> : IProxyBase<TTarget>
		where TTarget : ComponentBase
	{
		ElementBase? Element { get; set; }
	}

	internal class ComponentBaseProxy<TTarget> : ProxyBase<TTarget>, IComponentBaseProxy<TTarget>
		where TTarget : ComponentBase
	{
		public ComponentBaseProxy(PlayMaster owner, TTarget target) : base(owner, target)
		{
		}

		public ElementBase? Element { get => Target.Element; set => SetValue(x => x.Element, value); }
	}
}
