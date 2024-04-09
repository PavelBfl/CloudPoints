using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public sealed class Playground : Subject
	{
		public static Intersection.Context IntersectionContext { get; } = new Intersection.Context();

		public PlayerCharacter GetPlayerCharacterRequired() => Items.OfType<PlayerCharacter>().Single();

		public ICollection<Material> Items { get; } = new HashSet<Material>();
	}
}
