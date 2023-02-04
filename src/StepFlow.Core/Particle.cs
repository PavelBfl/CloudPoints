using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class Particle
	{
		public Particle(World? owner)
		{
			owner?.Particles.Add(this);
		}

		public World? Owner { get; internal set; }

		public World OwnerSafe => Owner ?? throw InvalidCoreException.CreateInvalidAccessOwner();
	}
}
