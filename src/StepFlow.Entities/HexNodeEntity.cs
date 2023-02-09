using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.Entities
{
	public class HexNodeEntity : ParticleEntity
	{
		public int Col { get; set; }

		public int Row { get; set; }

		[AllowNull]
		public ICollection<PieceEntity> Occupiers { get; set; }
	}
}
