using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISubjectProxy : IComponentController, IProxyBase<Subject>
	{
		string? Name { get; set; }

		IPlaygroundProxy Playground { get; }
	}
}
