using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Components
{
	internal class ContainerProxy<TTarget> : ProxyBase<TTarget>, IComponentController
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
