using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Entities
{
	public class ParticleEntity : EntityBase
	{
		public int OwnerId { get; set; }

		[AllowNull]
		public WorldEntity Owner { get; set; }
	}
}
