using StepFlow.Core.Components;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Enemy : Material
	{
		public Enemy(IContext context)
			: base(context)
		{
		}

		public Enemy(IContext context, EnemyDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Vision = original.Vision?.ToCollided(context);
			Cooldown = original.Cooldown;
			ReleaseItem = original.ReleaseItem;
		}

		private Collided? vision;

		public Collided? Vision { get => vision; set => SetComponent(ref vision, value); }

		public Collided GetVisionRequired() => PropertyRequired(Vision, nameof(Vision));

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }
	}
}
