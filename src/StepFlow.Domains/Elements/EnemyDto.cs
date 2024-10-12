using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Domains.Elements
{
	public sealed class EnemyDto : MaterialDto
	{
		public IList<Rectangle> Vision { get; } = new List<Rectangle>();

		public Scale Cooldown { get; set; }

		public ItemKind ReleaseItem { get; set; }

		public float? PatrolSpeed { get; set; }

		public Scale StunCooldown { get; set; }
	}
}
