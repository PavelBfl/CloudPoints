using System;

namespace StepFlow.Core
{
	public class Particle : Child
	{
		public Particle(Playground owner, Strength strength) : base(owner)
		{
			Strength = strength ?? throw new ArgumentNullException(nameof(strength));
		}

		public Strength Strength { get; }
	}
}
