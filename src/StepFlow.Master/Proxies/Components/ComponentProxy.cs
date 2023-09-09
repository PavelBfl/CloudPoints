using System.ComponentModel;

namespace StepFlow.Master.Proxies.Components
{
	public class ComponentProxy<TComponent> : ProxyBase<TComponent>, IComponentProxy
		where TComponent : Component
	{
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		public IContainerProxy Container => (IContainerProxy)Owner.CreateProxy(Target.Container);

		public IComponentProxy AddComponent(string name) => Container.AddComponent(name);

		public IComponentProxy? GetComponent(string name) => Container.GetComponent(name);

		public bool RemoveComponent(string name) => Container.RemoveComponent(name);
	}
}
