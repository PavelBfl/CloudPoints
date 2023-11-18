using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IItemProxy : IProxyBase<Item>, IDamageProxy
	{
		
	}

	internal class ItemProxy : MaterialProxy<Item>, IItemProxy
	{
		public ItemProxy(PlayMaster owner, Item target) : base(owner, target)
		{
		}

		public int Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public DamageKind Kind { get => Target.Kind; set => SetValue(x => x.Kind, value); }

		IDamage IReadOnlyProxyBase<IDamage>.Target => Target;
	}
}
