namespace StepFlow.Core.Elements
{
	public enum ObstructionKind
	{
		Single,
		Tiles,
	}

	public enum ObstructionView
	{
		None,
		DarkWall,
		Bricks,
		Boards,
	}

	public sealed class Obstruction : Material
	{
		public ObstructionKind Kind { get; set; }

		public ObstructionView View { get; set; }
	}
}
