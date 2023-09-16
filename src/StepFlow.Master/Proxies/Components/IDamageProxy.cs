using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IDamageProxy : IComponentProxy
	{
		float Value { get; set; }

		ICollection<string> Kind { get; }
	}
}
