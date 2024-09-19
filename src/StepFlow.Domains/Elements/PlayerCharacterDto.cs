using System.Collections.Generic;

namespace StepFlow.Domains.Elements
{
	public sealed class PlayerCharacterDto : MaterialDto
	{
		public Scale Cooldown { get; set; }

		public CharacterSkill MainSkill { get; set; }

		public CharacterSkill AuxiliarySkill { get; set; }

		public IList<ItemDto> Items { get; } = new List<ItemDto>();
	}
}
