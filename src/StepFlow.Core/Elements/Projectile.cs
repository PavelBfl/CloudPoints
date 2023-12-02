using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Projectile()
		{
		}

		public Subject? Creator { get; set; }

		public Damage? Damage { get; set; }

		public int CurrentPathIndex { get; set; }

		public IList<Course> Path { get; } = new List<Course>();
	}
}
