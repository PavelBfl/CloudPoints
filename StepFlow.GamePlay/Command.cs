using System;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Command : CommandBase
	{
		public Command(Context owner, Particle? target)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Target = target;
		}

		public Context Owner { get; }

		public Particle? Target { get; }
	}
}
