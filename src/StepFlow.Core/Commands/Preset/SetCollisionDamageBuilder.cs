namespace StepFlow.Core.Commands.Preset
{
	internal class SetCollisionDamageBuilder : IBuilder<Piece>
	{
		public float NewValue { get; set; }

		public ITargetingCommand<Piece> Build(Piece target) => new SetCollisionDamage(target, NewValue);

		public bool CanBuild(Piece target) => true;
	}
}
