using StepFlow.Core;

namespace StepFlow.Master
{
	public interface IPlaygroundCmd : IContainerCmd<Playground>
	{
		ICollectionCmd<IPieceCmd> Pieces { get; }

		IPlaceCmd Place { get; }

		INodeCmd CreateNode(int x, int y);

		IPieceCmd CreatePiece();

		ICollisionResultCmd GetCollision();
	}
}
