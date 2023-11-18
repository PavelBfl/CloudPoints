using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Item : Material, IDamage
	{
		public Item(Context owner) : base(owner)
		{
		}

		public int Value { get; set; }

		public DamageKind Kind { get; set; }
	}
}
