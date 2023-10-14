using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IComponentProxy
	{
		float Value { get; set; }

		float Max { get; set; }
		ICollection<IHandlerProxy> ValueChange { get; }

		void Add(float value);
	}
}
