using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;
using StepFlow.Domains;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public const int CELL_SIZE_DEFAULT = 50;

		public static Intersection.Context IntersectionContext { get; } = new Intersection.Context();

		public Playground()
		{
		}

		public Playground(PlaygroundDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Items.AddUniqueRange(original.Items.Select(CopyExtensions.ToMaterial));
		}

		public PlayerCharacter GetPlayerCharacterRequired() => Items.OfType<PlayerCharacter>().Single();

		public ICollection<Material> Items { get; } = new HashSet<Material>();
	}
}
