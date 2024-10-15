using System.Collections.Generic;

namespace StepFlow.Domains.Elements
{
	public sealed class PlayerCharacterDto : MaterialDto
	{
		public Scale Cooldown { get; set; }

		public int ActiveTarget { get; set; }

		public IList<ItemKind> Items { get; } = new List<ItemKind>();
	}
}
