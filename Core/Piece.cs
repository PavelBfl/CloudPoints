using System;

namespace StepFlow.Core
{
	public class Piece : Particle
	{
		public Piece(World owner, HexNode current)
			: base(owner)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
		}

		public HexNode Current { get; set; }
	}
}
