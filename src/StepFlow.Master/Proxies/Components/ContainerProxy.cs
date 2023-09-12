using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Components
{
	public class ContainerProxy<TTarget> : ProxyBase<TTarget>, IComponentController
		where TTarget : Container
	{
		public ContainerProxy(PlayMaster owner, TTarget target)
			: base(owner, target)
		{
		}

		public IComponentProxy AddComponent(string name)
		{
			Owner.TimeAxis.Add(new AddComponent(Target, Owner.CreateComponent(name), name));

			return ((IComponentController)this).GetComponentRequired(name);
		}

		public IComponentProxy? GetComponent(string? name) => CreateProxy(Target.Components[name]);

		private IComponentProxy? CreateProxy(IComponent? component)
		{
			if (component is IComponentProxy componentProxy)
			{
				return componentProxy;
			}
			else
			{
				return (IComponentProxy?)Owner.CreateProxy(component);
			}
		}

		public IEnumerable<IComponentProxy> GetComponents()
			=> Target.Components.OfType<IComponent>().Select(CreateProxy).OfType<IComponentProxy>();

		public bool RemoveComponent(string name)
		{
			if (Target.Components[name] is { })
			{
				Owner.TimeAxis.Add(new Reverse(new AddComponent(Target, Owner.CreateComponent(name), name)));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
