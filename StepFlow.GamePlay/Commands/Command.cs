using System;
using StepFlow.Common;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay.Commands
{
	public abstract class Command : ICommand
	{
		public Command(IParticle target)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Owner = target.Owner.Owner;
		}

		public Command(Context owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public abstract void Execute();

		public abstract void Revert();

		public Context Owner { get; }

		public IParticle? Target { get; }

		public IParticle TargetRequired => Target.PropertyRequired(nameof(Target));
	}
}
