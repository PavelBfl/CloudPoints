using System.ComponentModel;
using MoonSharp.Interpreter;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	public class ContainerProxy<TTarget> : ProxyBase<TTarget>
		where TTarget : Container
	{
		[MoonSharpHidden]
		public ContainerProxy(PlayMaster owner, TTarget target)
			: base(owner, target)
		{
		}

		public void AddComponent(string name) => Owner.TimeAxis.Add(new AddComponent(Target, Owner.CreateComponent(name), name));

		public void RemoveComponent(string name) => Owner.TimeAxis.Add(new Reverse(new AddComponent(Target, Owner.CreateComponent(name), name)));

		public IComponent GetComponent(string name) => Target.Components[name];
	}
}
