using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class ProjectileProxy : ComponentProxy<Projectile>, IProjectileProxy
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }
	}
}
