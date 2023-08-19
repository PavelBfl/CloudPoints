using System.ComponentModel;
using MoonSharp.Interpreter;

namespace StepFlow.Master.Proxies
{
	public class ComponentProxy<TComponent> : ProxyBase<TComponent>
		where TComponent : Component
	{
		[MoonSharpHidden]
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		public IComponent? GetComponent(string name) => Target.Container.Components[name];
	}
}
