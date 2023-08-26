using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public sealed class Collided : Component
	{
		public Bordered? Current { get; set; }

		public Bordered? Next { get; set; }

		public bool IsMoving { get; set; }

		public float Damage { get; set; }
	}
}
