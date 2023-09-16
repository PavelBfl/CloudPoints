using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface ICollisionDamageProxy : IComponentProxy
	{
		float Damage { get; set; }
		ICollection<string> Kind { get; }
	}
}
