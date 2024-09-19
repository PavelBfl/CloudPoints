using System.Collections.Generic;
using System.Linq;
using StepFlow.Domains;
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

		public override SubjectDto ToDto()
		{
			var result = new PlayerCharacterDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(PlayerCharacterDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Cooldown = Cooldown;
			container.MainSkill = MainSkill;
			container.AuxiliarySkill = AuxiliarySkill;
			container.Items.AddRange(Items.Select(x => x.ToDto()).Cast<ItemDto>());
		}
	}
}
