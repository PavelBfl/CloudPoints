using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class CollisionDamage : ComponentBase
	{
		public CollisionDamage(Playground owner) : base(owner)
		{
		}

		public float Damage { get; set; }

		public ICollection<string> Kind { get; } = new HashSet<string>();
	}
}
