using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IItemProxy : IMaterialProxy<Item>
	{
		ItemKind Kind { get; set; }

		Damage? DamageSettings { get; set; }

		int AttackCooldown { get; set; }
	}

	internal class ItemProxy : MaterialProxy<Item>, IItemProxy
	{
		public ItemProxy(PlayMaster owner, Item target) : base(owner, target)
		{
		}

		public ItemKind Kind { get => Target.Kind; set => SetValue(x => x.Kind, value); }

		public Damage? DamageSettings { get => Target.DamageSetting; set => SetValue(x => x.DamageSetting, value); }

		public int AttackCooldown { get => Target.AttackCooldown; set => SetValue(x => x.AttackCooldown, value); }
	}
}
