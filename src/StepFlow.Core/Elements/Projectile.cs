using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class Projectile : Material
	{
		public Projectile(Context owner) : base(owner)
		{
		}

		public IDamage? Damage { get; set; }
	}
}
