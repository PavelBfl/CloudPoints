using System.Collections.Generic;

namespace StepFlow.Entities
{
	public class HexNodeEntity : InheritorParticle
	{
		public int Col { get; set; }

		public int Row { get; set; }

		public ICollection<PieceEntity> Occupiers { get; set; }
	}
}
