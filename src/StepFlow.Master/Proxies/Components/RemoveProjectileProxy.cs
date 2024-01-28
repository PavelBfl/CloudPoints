using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Elements;

namespace StepFlow.Master.Proxies.Components
{
	public interface IRemoveProjectileProxy : IProxyBase<RemoveProjectile>, ITurnExecutor
	{
		Projectile? Projectile { get; set; }
	}

	internal sealed class RemoveProjectileProxy : ProxyBase<RemoveProjectile>, IRemoveProjectileProxy
	{
		public RemoveProjectileProxy(PlayMaster owner, RemoveProjectile target) : base(owner, target)
		{
		}

		public Projectile? Projectile { get => Target.Projectile; set => SetValue(x => x.Projectile, value); }

		Subject IReadOnlyProxyBase<Subject>.Target => Target;

		public void Execute()
		{
			if (Projectile is { } projectile)
			{
				var projectileProxy = (IProjectileProxy)Owner.CreateProxy(projectile);
				Owner.GetPlaygroundProxy().Projectiles.Remove(projectileProxy);
				projectileProxy.Body.Current = null;
				projectileProxy.Body.Next = null;
			}
		}
	}
}
