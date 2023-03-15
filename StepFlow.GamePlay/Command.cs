using System;
using StepFlow.TimeLine;

namespace StepFlow.GamePlay
{
	public class Command : CommandBase
	{
		public Command(IParticle target)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Owner = target.Owner;
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
				Owner.AxisTime.Registry(Owner.StaticCommands.Count, this);
			}
			else
			{
				Owner.AxisTime.Registry(Owner.StaticCommands.Count, this);
			}
		}

		public Context Owner { get; }

		public IParticle? Target { get; }
	}
}
