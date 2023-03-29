using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Particle
	{
		public IWorld? Owner { get; internal set; }

		public IWorld OwnerRequired => Owner ?? throw ExceptionBuilder.CreateInvalidAccessOwner();

		internal virtual void TakeStep()
		{
		}
	}
}
