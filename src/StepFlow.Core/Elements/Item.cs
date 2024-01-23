using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public enum ItemKind
	{
		None,
		Fire,
		Poison,
		Speed,

		AttackSpeed,
		AddStrength,
	}

	public sealed class Item : Material
	{
		public ItemKind Kind { get; set; }

		private Damage? damageSetting;

		public Damage? DamageSetting { get => damageSetting; set => SetComponent(ref damageSetting, value); }

		public int AttackCooldown { get; set; }

		public int AddStrength { get; set; }
	}
}
