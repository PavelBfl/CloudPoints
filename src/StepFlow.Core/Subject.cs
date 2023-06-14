using System;
using StepFlow.Core.Commands;
using StepFlow.Core.Exceptions;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Subject : IScheduled
	{
		public Subject(Playground owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Playground Owner { get; }

		public Axis<ITargetingCommand<object>> AxisTime => Owner.AxisTime;

		public long Time => Owner.Time;

		public object? Buffer { get => Owner.Buffer; set => Owner.Buffer = value; }

		protected void CheckInteraction(Subject? other)
		{
			if (other is { } && Owner != other.Owner)
			{
				throw ExceptionBuilder.CreatePairParticlesCanNotInteraction();
			}
		}

		protected void CheckInteractionRequired(Subject other)
		{
			if (other is null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			CheckInteraction(other);
		}
	}
}
