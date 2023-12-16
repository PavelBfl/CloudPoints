using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public enum ItemKind
	{
		None,
		Fire,
		Poison,
		Speed,
	}

	public sealed class Item : Material
	{
		public ItemKind Kind { get; set; }

		private Damage? damageSetting;

		public Damage? DamageSetting { get => damageSetting; set => SetComponent(ref damageSetting, value); }
	}
}
