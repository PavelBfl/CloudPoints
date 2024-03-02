using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		Subject? Creator { get; set; }

		Damage? Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public Subject? Creator { get => Target.Creator; set => SetValue(x => x.Creator, value); }

		public Damage? Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public override void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided)
		{
			if (Creator != otherMaterial && otherCollided.IsRigid)
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
