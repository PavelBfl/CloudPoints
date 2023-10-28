using System.Collections.Generic;
using StepFlow.Core.Components;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(Damage), typeof(DamageProxy), "DamageType")]
	public interface IDamageProxy : IComponentProxy
	{
		float Value { get; set; }

		ICollection<string> Kind { get; }
	}

	internal sealed class DamageProxy : ComponentProxy<Damage>, IDamageProxy
	{
		public DamageProxy(PlayMaster owner, Damage target) : base(owner, target)
		{
		}

		public float Value { get => Target.Value; set => SetValue(x => x.Value, value); }

		public ICollection<string> Kind => new CollectionProxy<string, ICollection<string>>(Owner, Target.Kind);
	}
}
