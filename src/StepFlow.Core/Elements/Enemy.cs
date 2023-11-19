using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Enemy : Material
	{
		public Collided? Vision { get; set; }

		public Scale? Cooldown { get; set; }

		public override IEnumerable<Collided> GetCollideds()
		{
			foreach (var item in base.GetCollideds())
			{
				yield return item;
			}

			if (Vision is { })
			{
				yield return Vision;
			}
		}
	}
}
