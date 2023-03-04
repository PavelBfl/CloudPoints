using System.Collections.Generic;

namespace StepFlow.Core.Collision
{
	public interface IPiecesCollision : IReadOnlyCollection<Piece>
	{
		bool Contains(Piece piece);
	}
}
