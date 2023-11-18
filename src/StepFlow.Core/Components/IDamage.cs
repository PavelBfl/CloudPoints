namespace StepFlow.Core.Components
{
	public enum DamageKind
	{
		None = 0,
		Fire = 1,
		Poison = 2,
	}

	public interface IDamage
	{
		int Value { get; set; }

		DamageKind Kind { get; set; }
	}

	public sealed class Damage : Subject, IDamage
	{
		public int Value { get; set; }

		public DamageKind Kind { get; set; }
	}
}
