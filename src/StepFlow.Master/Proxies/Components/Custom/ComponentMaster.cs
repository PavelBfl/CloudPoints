using System;
using System.ComponentModel;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class ComponentMaster : Component, IComponentProxy
	{
		public ComponentMaster(PlayMaster owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public PlayMaster Owner { get; }

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Container);
	}
}
