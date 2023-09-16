using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public sealed class DamageProxy : ComponentProxy<Damage>, IDamageProxy
	{
		public DamageProxy(PlayMaster owner, Damage target) : base(owner, target)
		{
		}

		public float Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public ICollection<string> Kind => new CollectionProxy<string, ICollection<string>>(Owner, Target.Kind);
	}
}
