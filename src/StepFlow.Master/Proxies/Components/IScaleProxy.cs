using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IScaleProxy : IProxyBase<IScale>
	{
		float Value { get; set; }

		float Max { get; set; }

		bool Freeze { get; set; }

		bool Add(float value);
	}
}
