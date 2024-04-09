using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;

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

		public Projectile? Projectile { get => Target.Projectile; set => SetValue(value); }

		Subject IProxyBase<Subject>.Target => Target;

		public void Execute()
		{
			if (Projectile is { } projectile)
			{
				var items = Owner.CreateCollectionUsedProxy(Owner.Playground.Items);
				items.Remove(projectile);
			}
		}
	}
}
