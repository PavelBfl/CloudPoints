namespace StepFlow.Domains.Elements
{
	public sealed class ItemDto : MaterialDto
	{
		public ItemKind Kind { get; set; }

		public Damage DamageSetting { get; set; }

		public int AttackCooldown { get; set; }

		public int AddStrength { get; set; }
	}
}
