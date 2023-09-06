using System;
using System.ComponentModel;
using MoonSharp.Interpreter;
using StepFlow.Master.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	public class ContainerProxy<TTarget> : ProxyBase<TTarget>, IComponentProvider
		where TTarget : Container
	{
		[MoonSharpHidden]
		public ContainerProxy(PlayMaster owner, TTarget target)
			: base(owner, target)
		{
		}

		[MoonSharpHidden]
		public T AddComponentProxy<T>(string name)
			where T : class
		{
			if (name is null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			AddComponent(name);
			return ((IComponentProvider)this).GetComponentProxyRequired<T>(name);
		}

		[MoonSharpHidden]
		public T GetComponentProxy<T>(string name)
			where T : class?
		{
			if (name is null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			var component = GetComponent(name);
			return (T)Owner.CreateProxy(component);
		}

		public void AddComponent(string name) => Owner.TimeAxis.Add(new AddComponent(Target, Owner.CreateComponent(name), name));

		public void RemoveComponent(string name) => Owner.TimeAxis.Add(new Reverse(new AddComponent(Target, Owner.CreateComponent(name), name)));

		public IComponent GetComponent(string name) => Target.Components[name];
	}
}
