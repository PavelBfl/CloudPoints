namespace StepFlow.Core.Commands.Preset
{
	public class CreateCommand : Command<Playground>
	{
		public CreateCommand(Playground playground)
			: base(playground)
		{
		}

		private object? OldBuffer { get; set; }

		private Piece? Piece { get; set; }

		public override void Execute()
		{
			Piece = new Piece(Target);
			OldBuffer = Target.Buffer;
			Target.Buffer = Piece;
		}

		public override void Revert()
		{
			Target.Buffer = OldBuffer;
			Piece = null;
		}
	}
}
