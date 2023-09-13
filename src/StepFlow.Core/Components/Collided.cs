using System.Collections.Generic;
using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public sealed class Collided : ComponentBase
	{
		public Collided(Playground owner) : base(owner)
		{
		}

		public Bordered? Current { get; set; }

		public Bordered? Next { get; set; }

		public bool IsMoving { get; set; }

		public ICollection<uint> Collision { get; } = new HashSet<uint>();
	}
}
