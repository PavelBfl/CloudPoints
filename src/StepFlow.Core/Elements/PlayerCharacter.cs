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
		private Scale? cooldown;

		public Scale? Cooldown { get => cooldown; set => SetComponent(ref cooldown, value); }

		public Scale GetCooldownRequired() => PropertyRequired(Cooldown, nameof(Cooldown));

		public CharacterSkill MainSkill { get; set; }

		public CharacterSkill AuxiliarySkill { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
