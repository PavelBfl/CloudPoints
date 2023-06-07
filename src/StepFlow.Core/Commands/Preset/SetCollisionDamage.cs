namespace StepFlow.Core.Commands.Preset
{
	internal class SetCollisionDamage : Command<Piece>
	{
		public SetCollisionDamage(Piece target, float newValue) : base(target)
		{
			NewValue = newValue;
		}

		private float OldValue { get; set; }

		private float NewValue { get; }

		public override void Execute()
		{
			OldValue = Target.CollisionDamage;
			Target.CollisionDamage = NewValue;
		}

		public override void Revert()
		{
			Target.CollisionDamage = OldValue;
		}
	}
}
