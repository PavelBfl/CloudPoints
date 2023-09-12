using System.ComponentModel;

namespace StepFlow.Master.Proxies.Components
{
	public class ComponentProxy<TComponent> : ProxyBase<TComponent>, IComponentProxy
		where TComponent : Component
	{
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Target.Container);

		public IComponentProxy AddComponent(string name) => Subject.AddComponent(name);

		public IComponentProxy? GetComponent(string? name) => Subject.GetComponent(name);

		public bool RemoveComponent(string name) => Subject.RemoveComponent(name);

		public void HandleEvent(string? name, object? args = null)
		{
			if (name is { } && Subject.Playground.Handlers.TryGetValue(name, out var handler))
			{
				handler.Handle(this, name, args);
			}
		}
	}
}
