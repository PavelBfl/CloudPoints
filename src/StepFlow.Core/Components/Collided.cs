using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class Collided : ComponentBase
	{
		public Collided(Playground owner) : base(owner)
		{
			Collision = new Event(Owner);
		}

		public Bordered? Current { get; set; }

		public Bordered? Next { get; set; }

		public bool IsMoving { get; set; }

		public ICollection<IComponentChild> Collision { get; }
	}
}
