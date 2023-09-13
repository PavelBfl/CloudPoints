using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

			var component = ((IComponentController)this).GetComponentRequired(name);
			return component;
		}

		public IComponentProxy? GetComponent(string? name) => CreateProxy(Target.Components[name]);

		[return: NotNullIfNotNull("component")]
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
			if (Target.Components[name] is { } component)
			{
				Owner.TimeAxis.Add(new Reverse(new AddComponent(Target, component, name)));
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
