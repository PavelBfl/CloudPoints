using StepFlow.Core.Components;
using StepFlow.Domains;
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

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new EnemyDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(EnemyDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Vision = (CollidedDto?)Vision?.ToDto();
			container.Cooldown = Cooldown;
			container.ReleaseItem = ReleaseItem;
		}

		public override void Enable()
		{
			base.Enable();

			Vision?.Enable();
		}

		public override void Disable()
		{
			base.Disable();

			Vision?.Disable();
		}
	}
}
