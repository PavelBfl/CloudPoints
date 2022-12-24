namespace StepFlow.Core
{
	public class HexNode : Particle
	{
		private const string VIEW_FORMAT = "{0}:{1}";

		public HexNode(World owner, int col, int row)
			: base(owner)
		{
			Col = col;
			Row = row;
		}

		public int Col { get; }

		public int Row { get; }

		public override string ToString() => string.Format(VIEW_FORMAT, Col, Row);
	}
}
