using System;
using System.Collections.Generic;
using StepFlow.Core.Commands;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Particle : Child
	{
		public Particle(Playground owner, Strength strength) : base(owner)
		{
			Strength = strength ?? throw new ArgumentNullException(nameof(strength));
		}

		public Strength Strength { get; }

		public IList<ITargetingCommand<Particle>> Commands { get; } = new List<ITargetingCommand<Particle>>();
	}
}
