using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Damage Damage { get; set; }

		public bool Reusable { get; set; }

		public ICollection<Subject> Immunity { get; } = new HashSet<Subject>();
	}
}
