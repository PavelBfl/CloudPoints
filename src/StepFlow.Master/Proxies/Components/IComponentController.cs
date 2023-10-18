using System;
using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface IComponentController
	{
		IComponentProxy AddComponent(string componentType, string? name = null);

		IHandlerProxy AddHandler(string reference, bool disposable = false)
		{
			var result = (IHandlerProxy)AddComponent(Master.Components.Types.HANDLER);
			result.Reference = reference;
			result.Disposable = disposable;
			return result;
		}

		void RemoveComponent(IComponentProxy component);

		IEnumerable<IComponentProxy> GetComponents();

		IComponentProxy? GetComponent(string? name);

		IComponentProxy GetComponentRequired(string name) => GetComponent(name) ?? throw new InvalidOperationException();
	}
}
