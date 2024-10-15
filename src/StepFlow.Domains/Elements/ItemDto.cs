using System.Collections.Generic;

namespace StepFlow.Domains.Elements
{
	public sealed class ItemDto : MaterialDto
	{
		public ItemKind Kind { get; set; }

		public IList<ProjectileDto> Projectiles { get; } = new List<ProjectileDto>();

		public int AttackCooldown { get; set; }

		public int AddStrength { get; set; }
	}
}
