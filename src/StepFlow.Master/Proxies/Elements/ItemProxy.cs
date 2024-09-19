using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IItemProxy : IMaterialProxy<Item>
	{
		ItemKind Kind { get; set; }

		Damage? DamageSettings { get; set; }

		int AttackCooldown { get; set; }

		int AddStrength { get; set; }
	}

	internal class ItemProxy : MaterialProxy<Item>, IItemProxy
	{
		public ItemProxy(PlayMaster owner, Item target) : base(owner, target)
		{
		}

		public ItemKind Kind { get => Target.Kind; set => SetValue(value); }

		public Damage? DamageSettings { get => Target.DamageSetting; set => SetValue(value); }

		public int AttackCooldown { get => Target.AttackCooldown; set => SetValue(value); }

		public int AddStrength { get => Target.AddStrength; set => SetValue(value); }
	}
}
