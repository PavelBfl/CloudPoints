using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Material
	{
		public IScale? Cooldown { get; set; }

		public IList<Item> Items { get; } = new List<Item>();
	}
}
