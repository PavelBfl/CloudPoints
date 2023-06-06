namespace StepFlow.Core.Commands
{
	internal class MoveCommandBuilder : IBuilder<Piece>
	{
		public Node Next { get; set; }

		public ITargetingCommand<Piece> Build(Piece target) => new MoveCommand(target, Next);

		public bool CanBuild(Piece target) => true;
	}
}
