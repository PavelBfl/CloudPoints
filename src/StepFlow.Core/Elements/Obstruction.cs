namespace StepFlow.Core.Elements
{
	public enum ObstructionKind
	{
		Single,
		Tiles,
	}

	public sealed class Obstruction : Material
	{
		public ObstructionKind Kind { get; set; }
	}
}
