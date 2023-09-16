using System;
using System.ComponentModel;
using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class ComponentMaster : Component, IComponentProxy, IComponentChild
	{
		public ComponentMaster(PlayMaster owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			Id = Owner.Playground.GenerateId();
			Owner.Playground.Register(this);
		}

		public PlayMaster Owner { get; }

		Playground IChild.Owner => ((IChild)Container).Owner;

		public uint Id { get; }

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Container);

		public string? Name => Site?.Name;

		public IComponent Target => this;
	}
}
