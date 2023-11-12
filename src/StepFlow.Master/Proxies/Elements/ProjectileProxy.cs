using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IProjectileProxy : IProxyBase<Projectile>, IMaterialProxy
	{
		IDamageProxy? Damage { get; set; }
	}

	internal sealed class ProjectileProxy : MaterialProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public IDamageProxy? Damage
		{
			get => (IDamageProxy?)Owner.CreateProxy(Target.Damage);
			set => SetValue(x => x.Damage, value?.Target);
		}

		public override void Collision(ICollidedProxy other)
		{
			base.Collision(other);

			if (other is IMaterialProxy { Strength: { } strength } && Damage is { } damage)
			{
				strength.Add(-damage.Value);
			}
		}
	}
}
