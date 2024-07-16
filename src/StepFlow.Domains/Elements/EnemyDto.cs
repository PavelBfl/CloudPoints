using StepFlow.Domains.Components;

namespace StepFlow.Domains.Elements
{
	public sealed class EnemyDto : MaterialDto
	{
		public CollidedDto? Vision { get; set; }

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }
	}
}
