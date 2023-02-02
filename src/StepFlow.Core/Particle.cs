using System;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Particle
	{
		public Particle(World? owner)
		{
			Owner = owner;
			if (Owner is { })
			{
				Owner.Particles.Add(this);
			}
		}

		public World? Owner { get; internal set; }

		public World OwnerSafe => Owner ?? throw InvalidCoreException.CreateInvalidAccessOwner();
	}
}
