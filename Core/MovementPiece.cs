using System;
using TimeLine;

namespace Core
{
	public class MovementPiece : Piece
	{
		public MovementPiece(World owner, HexNode current)
			: base(owner)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
		}

		public HexNode Current { get; private set; }

		public void MoveTo(HexNode node)
		{
			Owner.TimeAxis.Registry(Owner.TimeAxis.Current + 1, new MoveCommand(this, node));
		}

		private class MoveCommand : CommandBase
		{
			public MoveCommand(MovementPiece owner, HexNode nextNode)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));
			}

			private MovementPiece Owner { get; }

			private HexNode NextNode { get; }

			public override void Execute()
			{
				Owner.Current = NextNode;
			}
		}
	}
}
