using System.Numerics;
using StepFlow.Core.Elements;
using StepFlow.Domains.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Damage Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Damage Damage { get => Target.Damage; set => SetValue(value); }

		protected override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (
				Target != otherMaterial &&
				otherCollided.IsBody &&
				!Target.Immunity.Contains(otherMaterial) &&
				otherCollided.Material.Body.IsRigid
			)
			{
				base.Collision(thisCollided, otherMaterial, otherCollided);

				var otherMaterialProxy = (IMaterialProxy<Material>)Owner.CreateProxy(otherMaterial);
				otherMaterialProxy.Strength -= Damage.Value;

				if (otherMaterial is Enemy enemy)
				{
					var enemyProxy = (IEnemyProxy)Owner.CreateProxy(enemy);
					enemyProxy.StunCooldown = Scale.CreateByMax(TimeTick.FromSeconds(1));
				}

				if (Damage.Push != Vector2.Zero && otherMaterial.Weight < Material.MAX_WEIGHT)
				{
					otherMaterialProxy.Course += Damage.Push;
				}

				switch (Target.Reusable)
				{
					case ReusableKind.None:
						Owner.GetPlaygroundItemsProxy().Remove(Target);
						break;
					case ReusableKind.Save:
						Owner.CreateCollectionProxy(Target.Immunity).Add(otherMaterial);
						break;
				}
			}
		}
	}
}
