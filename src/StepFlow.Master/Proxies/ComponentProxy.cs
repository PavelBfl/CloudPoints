using System.ComponentModel;
using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public class ComponentProxy<TComponent> : ProxyBase<TComponent>, IComponentProvider
		where TComponent : Component
	{
		[MoonSharpHidden]
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		[MoonSharpHidden]
		public T AddComponentProxy<T>(string name)
			where T : class
			=> ((IComponentProvider)Owner.CreateProxy(Target.Container)).AddComponentProxy<T>(name);

		[MoonSharpHidden]
		public T GetComponentProxy<T>(string name)
			where T : class?
			=> ((IComponentProvider)Owner.CreateProxy(Target.Container)).GetComponentProxy<T>(name);

		public IComponent? GetComponent(string name) => Target.Container.Components[name];
	}
}
