using System.Collections.Generic;
using System.Linq;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Material
	{
		public PlayerCharacter(IContext context)
			: base(context)
		{
		}

		public PlayerCharacter(IContext context, PlayerCharacterDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Cooldown = original.Cooldown;
			MainSkill = original.MainSkill;
			AuxiliarySkill = original.AuxiliarySkill;
			Items.AddRange(original.Items.Select(x => CopyExtensions.ToItem(x, Context)));
		}

		public Scale Cooldown { get; set; }

		public CharacterSkill MainSkill { get; set; }

		public CharacterSkill AuxiliarySkill { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
