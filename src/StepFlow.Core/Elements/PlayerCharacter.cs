using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public enum CharacterSkill
	{
		Projectile,
		Arc,
		Push,
		Dash,
	}

	public sealed class PlayerCharacter : Material
	{
		public Scale Cooldown { get; set; }

		public CharacterSkill MainSkill { get; set; }

		public CharacterSkill AuxiliarySkill { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
