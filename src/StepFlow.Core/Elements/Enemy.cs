using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Enemy : Material
	{
		public Enemy(Context owner) : base(owner)
		{
		}

		public ICollided? Vision { get; set; }

		public IScale? Cooldown { get; set; }

		public override IEnumerable<Subject> GetContent()
		{
			foreach (var item in base.GetContent())
			{
				yield return item;
			}

			if (Vision is { })
			{
				yield return (Subject)Vision;
			}
		}
	}
}
