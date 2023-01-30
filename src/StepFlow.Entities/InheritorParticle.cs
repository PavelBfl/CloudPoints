using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Entities
{
	public class InheritorParticle : EntityBase
	{
		public int ParticleId { get; set; }

		[AllowNull]
		public ParticleEntity Particle { get; set; }
	}
}
