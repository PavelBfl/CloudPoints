namespace StepFlow.Entities
{
	public class InheritorParticle : EntityBase
	{
		public int ParticleId { get; set; }
		public ParticleEntity Particle { get; set; }
	}
}
