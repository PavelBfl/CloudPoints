using System;
using System.ComponentModel;

namespace StepFlow.Core
{
	public sealed class Subject : Container, IChild
	{
		public Subject(Playground owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Id = Owner.GenerateId();
			Owner.Register(this);
		}

		public Playground Owner { get; }

		public uint Id { get; }

		public string? Name { get; set; }
	}
}
