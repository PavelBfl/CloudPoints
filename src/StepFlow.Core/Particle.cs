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

		private double strength;
		public double Strength
		{
			get => strength;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				strength = Math.Min(value, StrengthMax);
			}
		}

		private double strengthMax;
		public double StrengthMax
		{
			get => strengthMax;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				strengthMax = value;
				strength = Math.Min(Strength, StrengthMax);
			}
		}
	}
}
