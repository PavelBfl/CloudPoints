using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IMaterialProxy<Projectile>
	{
		IProxyBase<Subject>? Creator { get; set; }

		IDamageProxy? Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public IProxyBase<Subject>? Creator
		{
			get => (IProxyBase<Subject>?)Owner.CreateProxy(Target.Creator);
			set => SetValue(x => x.Creator, value?.Target);
		}

		public IDamageProxy? Damage
		{
			get => (IDamageProxy?)Owner.CreateProxy(Target.Damage);
			set => SetValue(x => x.Damage, value?.Target);
		}

		public override void OnTick()
		{
			if (!Scheduler.Queue.Any())
			{
				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
			}
		}

		public override void Collision(ICollidedProxy thisCollided, IMaterialProxy<Material> otherMaterial, ICollidedProxy otherCollided)
		{
			if (Creator?.Target != otherMaterial.Target)
			{
				if (otherMaterial.Strength is { } strength)
				{
					strength.Add(-Damage.Value);
				}

				Owner.GetPlaygroundProxy().Projectiles.Remove(this);
			}
		}
	}
}
