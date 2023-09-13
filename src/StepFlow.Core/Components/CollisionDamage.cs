using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public sealed class CollisionDamage : ComponentBase
	{
		public CollisionDamage(Playground owner) : base(owner)
		{
		}

		public float Damage { get; set; }
	}
}
