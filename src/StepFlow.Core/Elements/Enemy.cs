using StepFlow.Domains;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;

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

			Vision = Shape.Create(original.Vision);
			Cooldown = original.Cooldown;
			ReleaseItem = original.ReleaseItem;
			PatrolSpeed = original.PatrolSpeed;
			StunCooldown = original.StunCooldown;
		}

		private Shape? vision;

		public Shape? Vision { get => vision; set => SetShape(ref vision, value, nameof(Vision)); }

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }

		public float? PatrolSpeed { get; set; }

		public Scale StunCooldown { get; set; }

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

			container.Vision.Reset(Vision);
			container.Cooldown = Cooldown;
			container.ReleaseItem = ReleaseItem;
			container.PatrolSpeed = PatrolSpeed;
			container.StunCooldown = StunCooldown;
		}

		public override void Enable()
		{
			base.Enable();

			if (!IsEnable)
			{
				if (Vision is { })
				{
					Context.IntersectionContext.Add(Vision); 
				}
			}
		}

		public override void Disable()
		{
			base.Disable();

			Vision?.Disable();
		}
	}
}
