using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Subject? Creator { get; set; }

		Damage Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Subject? Creator { get => Target.Creator; set => SetValue(value); }

		public Damage Damage { get => Target.Damage; set => SetValue(value); }

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (Creator != otherMaterial && otherCollided.Collided.IsRigid)
			{
				if (otherMaterial.Strength is { } strength)
				{
					var strengthProxy = (IScaleProxy)Owner.CreateProxy(strength);
					strengthProxy.Add(-Damage.Value);
				}

				var projectilesProxy = CreateListProxy(Owner.Playground.Projectiles);
				projectilesProxy.Remove(Target);

				var bodyProxy = (ICollidedProxy)Owner.CreateProxy(Body);
				bodyProxy.Clear();
			}
		}
	}
}
