using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Material
	{
		public Scale? Cooldown { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
