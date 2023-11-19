using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Subject? Creator { get; set; }

		public Damage? Damage { get; set; }
	}
}
