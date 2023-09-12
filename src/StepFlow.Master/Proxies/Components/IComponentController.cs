using System;
using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentController
	{
		IComponentProxy AddComponent(string name);

		bool RemoveComponent(string name);

		IEnumerable<IComponentProxy> GetComponents();

		IComponentProxy? GetComponent(string? name);

		IComponentProxy GetComponentRequired(string name) => GetComponent(name) ?? throw new InvalidOperationException();
	}
}
