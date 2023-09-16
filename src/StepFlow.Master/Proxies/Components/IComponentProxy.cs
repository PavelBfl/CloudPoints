using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentProxy : IProxyBase<IComponentChild>
	{
		string? Name { get; }

		ISubjectProxy Subject { get; }
	}
}
