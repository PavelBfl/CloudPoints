using System.ComponentModel;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentProxy : IProxyBase<IComponent>
	{
		string? Name { get; }

		ISubjectProxy Subject { get; }
	}
}
