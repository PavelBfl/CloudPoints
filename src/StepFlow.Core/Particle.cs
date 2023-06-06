using System;

namespace StepFlow.Core
{
	public class Particle : Child
	{
		public Particle(Playground owner) : base(owner)
		{
		}

		public Strength Strength { get; } = new Strength();
	}
}
