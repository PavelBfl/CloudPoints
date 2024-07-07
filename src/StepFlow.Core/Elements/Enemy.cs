using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Enemy : Material
	{
		private Collided? vision;

		public Collided? Vision { get => vision; set => SetComponent(ref vision, value); }

		public Collided GetVisionRequired() => PropertyRequired(Vision, nameof(Vision));

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }
	}
}
