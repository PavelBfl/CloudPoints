using System.Collections.Generic;
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
			ActiveTarget = original.ActiveTarget;
			Items.AddRange(original.Items);
		}

		public Scale Cooldown { get; set; }

		public int ActiveTarget { get; set; }

		public IList<ItemKind> Items { get; } = new List<ItemKind>();

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
			container.ActiveTarget = ActiveTarget;
			container.Items.AddRange(Items);
		}
	}
}
