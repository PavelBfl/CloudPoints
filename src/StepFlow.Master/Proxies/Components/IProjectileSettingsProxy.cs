using System.Collections.Generic;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface IProjectileSettingsProxy
	{
		Course Course { get; set; }

		int Size { get; set; }

		float Damage { get; set; }

		ICollection<string> Kind { get; }
	}
}
