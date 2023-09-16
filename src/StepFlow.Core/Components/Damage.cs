using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class Damage : ComponentBase
	{
		public Damage(Playground owner) : base(owner)
		{
		}

		public float Value { get; set; }

		public ICollection<string> Kind { get; } = new HashSet<string>();
	}
}
