namespace StepFlow.Core.Components
{
	public enum DamageKind
	{
		None = 0,
		Fire = 1,
		Poison = 2,
	}

	public sealed class Damage : ComponentBase
	{
		public int Value { get; set; }

		public DamageKind Kind { get; set; }
	}
}
