using System;
using StepFlow.Common;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay.Commands
{
	public abstract class Command : CommandBase
	{
		public Command(IParticle target)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Owner = target.Owner.Owner;
			SetAxis();
		}

		public Command(Context owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			SetAxis();
		}

		private void SetAxis()
		{
			if (Target is null)
			{
				Owner.AxisTime.Registry(Owner.StaticCommands.Count + 1, this);
			}
			else
			{
				Owner.AxisTime.Registry(Target.Commands.Count + 1, this);
			}
		}

		public Context Owner { get; }

		public IParticle? Target { get; }

		public IParticle TargetRequired => Target.PropertyRequired(nameof(Target));
	}
}
