using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public interface IScaleHandler : IIdentity
	{
		void ValueChange(IScaleProxy component, string name);
	}
}
