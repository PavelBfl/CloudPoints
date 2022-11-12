namespace StepFlow.Core
{
	public class HexNode : Particle
	{
		public HexNode(World owner, int col, int row)
			: base(owner)
		{
			Col = col;
			Row = row;
		}

		public int Col { get; }

		public int Row { get; }
	}
}
