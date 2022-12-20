namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner)
			: base(owner)
		{
		}

		public HexNode? Current { get; set; }
	}
}
