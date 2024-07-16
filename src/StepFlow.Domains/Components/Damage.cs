using System.Numerics;

namespace StepFlow.Domains.Components
{
	public struct Damage
	{
		public int Value { get; set; }

		public Vector2 Push { get; set; }

		public DamageKind Kind { get; set; }
	}
}
