using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Enemy : Material
	{
		private Collided? vision;

		public Collided? Vision { get => vision; set => SetComponent(ref vision, value); }

		public Collided GetVisionRequired() => PropertyRequired(Vision, nameof(Vision));

		private Scale? cooldown;

		public Scale? Cooldown { get => cooldown; set => SetComponent(ref cooldown, value); }

		public Scale GetCooldownRequired() => PropertyRequired(Cooldown, nameof(Cooldown));

		public ItemKind ReleaseItem { get; set; }
	}
}
