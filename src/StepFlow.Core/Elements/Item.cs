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

		public Damage? DamageSetting { get; set; }
	}
}
