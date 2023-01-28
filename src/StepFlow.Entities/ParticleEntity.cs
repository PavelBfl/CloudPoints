namespace StepFlow.Entities
{
	public class ParticleEntity : EntityBase
	{
		public int OwnerId { get; set; }

		public WorldEntity Owner { get; set; }
	}
}
