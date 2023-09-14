using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface IScaleHandler : IChild
	{
		void ValueChange(IScaleProxy component, string name);
	}
}
