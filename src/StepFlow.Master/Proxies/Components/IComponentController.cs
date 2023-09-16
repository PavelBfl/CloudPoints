using System;
using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentController
	{
		IComponentProxy AddComponent(string componentType, string? name = null);

		void RemoveComponent(IComponentProxy component);

		IEnumerable<IComponentProxy> GetComponents();

		IComponentProxy? GetComponent(string? name);

		IComponentProxy GetComponentRequired(string name) => GetComponent(name) ?? throw new InvalidOperationException();
	}
}
