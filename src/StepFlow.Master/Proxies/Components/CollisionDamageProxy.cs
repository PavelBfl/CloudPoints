using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class CollisionDamageProxy : ComponentProxy<CollisionDamage>, ICollisionDamageProxy
	{
		public CollisionDamageProxy(PlayMaster owner, CollisionDamage target) : base(owner, target)
		{
		}

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }
	}
}
