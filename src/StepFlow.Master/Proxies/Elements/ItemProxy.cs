using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IItemProxy : IMaterialProxy<Item>
	{
		IDamageProxy? DamageSettings { get; set; }
	}

	internal class ItemProxy : MaterialProxy<Item>, IItemProxy
	{
		public ItemProxy(PlayMaster owner, Item target) : base(owner, target)
		{
		}

		public IDamageProxy? DamageSettings
		{
			get => (IDamageProxy?)Owner.CreateProxy(Target.DamageSetting);
			set => SetValue(x => x.DamageSetting, value?.Target);
		}
	}
}
