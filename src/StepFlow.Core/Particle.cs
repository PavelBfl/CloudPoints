using System;
using System.Diagnostics;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Particle
	{
		public Particle(World? owner)
		{
			owner?.Particles.Add(this);
		}

		private World? owner;

		public virtual World? Owner
		{
			get => owner;
			set
			{
				if (Owner != value)
				{
					if (Owner is { })
					{
						Owner.Particles.RemoveForce(this);
					}

					owner = value;

					if (Owner is { })
					{
						Owner.Particles.AddForce(this);
					}
				}
			}
		}

		public World OwnerRequired => Owner ?? throw ExceptionBuilder.CreateInvalidAccessOwner();

		internal virtual void TakeStep()
		{
		}
	}
}
