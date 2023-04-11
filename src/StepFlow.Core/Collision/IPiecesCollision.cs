using System.Collections.Generic;

namespace StepFlow.Core.Collision
{
	public interface IPiecesCollision<TPiece> : IReadOnlyCollection<TPiece>
		where TPiece : Piece
	{
		bool Contains(TPiece piece);
	}
}
