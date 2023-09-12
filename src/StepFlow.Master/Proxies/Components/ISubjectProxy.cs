using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISubjectProxy : IComponentController, IProxyBase<Subject>
	{
		IPlaygroundProxy Playground { get; }
	}
}
