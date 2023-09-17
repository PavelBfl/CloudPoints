using System.Collections.Generic;

namespace StepFlow.Core.Components
{
	public sealed class ProjectileSettings : ComponentBase
	{
		public ProjectileSettings(Playground owner) : base(owner)
		{
		}

		public Course Course { get; set; }

		public int Size { get; set; }

		public float Damage { get; set; }

		public ICollection<string> Kind { get; } = new HashSet<string>();
	}
}
