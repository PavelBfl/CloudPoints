using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class Scale : ComponentBase
	{
		public Scale(Playground owner) : base(owner)
		{
		}

		public float Value { get; set; }

		public float Max { get; set; }

		public ICollection<uint> ValueChange { get; } = new HashSet<uint>();
	}
}
