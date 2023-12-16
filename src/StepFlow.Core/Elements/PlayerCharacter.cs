using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Material
	{
		private Scale? cooldown;

		public Scale? Cooldown { get => cooldown; set => SetComponent(ref cooldown, value); }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
