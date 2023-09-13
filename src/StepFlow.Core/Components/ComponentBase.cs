using System;
using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public class ComponentBase : Component, IIdentity
	{
		public ComponentBase(Playground owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Id = Owner.GenerateId();
			Owner.Register(this);
		}

		public Playground Owner { get; }

		public uint Id { get; }
	}
}
