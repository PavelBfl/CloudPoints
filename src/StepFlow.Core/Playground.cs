using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;
using StepFlow.Domains;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public const int CELL_SIZE_DEFAULT = 50;

		public Playground(IContext context)
			: base(context)
		{
		}

		public Playground(IContext context, PlaygroundDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Items.AddUniqueRange(original.Items.Select(x => x.ToMaterial(Context)));
		}

		public PlayerCharacter GetPlayerCharacterRequired() => Items.OfType<PlayerCharacter>().Single();

		public ICollection<Material> Items { get; } = new HashSet<Material>();
	}
}
