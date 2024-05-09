using System.Numerics;

namespace StepFlow.Core.Components
{
	public enum DamageKind
	{
		None = 0,
		Fire = 1,
		Poison = 2,
	}

	public struct Damage
	{
		public int Value { get; set; }

		public Vector2 Push { get; set; }

		public DamageKind Kind { get; set; }
	}
}
