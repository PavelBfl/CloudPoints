using System;
using System.ComponentModel;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Subject : Container
	{
		public Subject(Playground owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Playground Owner { get; }

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
