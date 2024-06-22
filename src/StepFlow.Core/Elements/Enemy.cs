using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public enum Strategy
	{
		None,
		CW,
		CWW,
		Reflection
	}

	public enum AttackStrategy
	{
		Target,
		Left,
		Top,
		Right,
		Bottom,
	}

	public sealed class Enemy : Material
	{
		private Collided? vision;

		public Collided? Vision { get => vision; set => SetComponent(ref vision, value); }

		public Collided GetVisionRequired() => PropertyRequired(Vision, nameof(Vision));

		private Scale? cooldown;

		public Scale? Cooldown { get => cooldown; set => SetComponent(ref cooldown, value); }

		public Scale GetCooldownRequired() => PropertyRequired(Cooldown, nameof(Cooldown));

		public Strategy Strategy { get; set; }

		public AttackStrategy AttackStrategy { get; set; }

		public ItemKind ReleaseItem { get; set; }
	}
}
