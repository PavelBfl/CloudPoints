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

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (Creator != otherMaterial.Target && otherCollided.IsRigid)
			{
				if (otherMaterial.Strength is { } strength)
				{
					var strengthProxy = (IScaleProxy)Owner.CreateProxy(strength);
					strengthProxy.Add(-Damage.Value);
				}

				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
				Body.Current = null;
				Body.Next = null;
			}
		}
	}
}
