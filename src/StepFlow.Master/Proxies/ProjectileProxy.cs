using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies
{
	public sealed class ProjectileProxy : ProxyBase<Projectile>
	{
		public ProjectileProxy(PlayMaster owner, Projectile target) : base(owner, target)
		{
		}

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }
	}
}
