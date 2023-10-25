using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Components
{
	public interface IContainerProxy
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

	internal class ContainerProxy<TTarget> : ProxyBase<TTarget>, IContainerProxy
		where TTarget : Container
	{
		public ContainerProxy(PlayMaster owner, TTarget target)
			: base(owner, target)
		{
		}

		public IComponentProxy AddComponent(string componentType, string? name = null)
		{
			var component = Owner.CreateComponent(componentType);
			Owner.TimeAxis.Add(new AddComponent(Target, component, name));

			return (IComponentProxy)Owner.CreateProxy(component);
		}

		public IComponentProxy? GetComponent(string? name) => (IComponentProxy)Owner.CreateProxy(Target.Components[name]);

		public IEnumerable<IComponentProxy> GetComponents()
			=> Target.Components.OfType<IComponent>().Select(Owner.CreateProxy).OfType<IComponentProxy>();

		public void RemoveComponent(IComponentProxy component)
		{
			Owner.TimeAxis.Add(new Reverse(new AddComponent(Target, component.Target, component.Name)));
		}
	}
}
