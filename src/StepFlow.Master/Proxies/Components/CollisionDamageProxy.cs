using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class CollisionDamageProxy : ComponentProxy<CollisionDamage>, ICollisionDamageProxy
	{
		public CollisionDamageProxy(PlayMaster owner, CollisionDamage target) : base(owner, target)
		{
		}

		public float Damage { get => Target.Damage; set => SetValue(x => x.Damage, value); }

		public ICollection<string> Kind => new CollectionProxy<string, ICollection<string>>(Owner, Target.Kind);
	}
}
