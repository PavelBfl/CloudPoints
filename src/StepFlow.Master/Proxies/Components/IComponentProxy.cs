using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentProxy : IComponentController
	{
		ISubjectProxy Subject { get; }

		IComponentProxy IComponentController.AddComponent(string name) => Subject.AddComponent(name);

		bool IComponentController.RemoveComponent(string name) => Subject.RemoveComponent(name);

		IEnumerable<IComponentProxy> IComponentController.GetComponents() => Subject.GetComponents();

		IComponentProxy? IComponentController.GetComponent(string? name) => Subject.GetComponent(name);
	}
}
