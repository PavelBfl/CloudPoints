using System.Collections.Generic;
using System.Linq;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Material
	{
		public PlayerCharacter()
		{
		}

		public PlayerCharacter(PlayerCharacterDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Cooldown = original.Cooldown;
			MainSkill = original.MainSkill;
			AuxiliarySkill = original.AuxiliarySkill;
			Items.AddRange(original.Items.Select(CopyExtensions.ToItem));
		}

		public Scale Cooldown { get; set; }

		public CharacterSkill MainSkill { get; set; }

		public CharacterSkill AuxiliarySkill { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
