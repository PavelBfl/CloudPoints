using System;
using System.ComponentModel;
using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class ComponentMaster : Component, IComponentProxy, IIdentity
	{
		public ComponentMaster(PlayMaster owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			Id = Owner.Playground.GenerateId();
			Owner.Playground.Register(this);
		}

		public PlayMaster Owner { get; }

		public uint Id { get; }

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Container);
	}
}
