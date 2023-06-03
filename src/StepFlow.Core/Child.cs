using System;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Child
	{
		public Child(Playground owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Playground Owner { get; }

		protected void CheckInteraction(Child? other)
		{
			if (other is { } && Owner != other.Owner)
			{
				throw ExceptionBuilder.CreatePairParticlesCanNotInteraction();
			}
		}

		protected void CheckInteractionRequired(Child other)
		{
			if (other is null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			CheckInteraction(other);
		}
	}
}
