using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public sealed class Item : Material
	{
		public Item(IContext context)
			: base(context)
		{
		}

		public Item(IContext context, ItemDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Kind = original.Kind;
			DamageSetting = original.DamageSetting;
			AttackCooldown = original.AttackCooldown;
			AddStrength = original.AddStrength;
		}

		public ItemKind Kind { get; set; }

		public Damage DamageSetting { get; set; }

		public int AttackCooldown { get; set; }

		public int AddStrength { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new ItemDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(ItemDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Kind = Kind;
			container.DamageSetting = DamageSetting;
			container.AttackCooldown = AttackCooldown;
			container.AddStrength = AddStrength;
		}
	}
}
