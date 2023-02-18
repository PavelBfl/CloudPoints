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

		public World? Owner { get; internal set; }

		public World OwnerSafe => Owner ?? throw InvalidCoreException.CreateInvalidAccessOwner();

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
