using System;
using CloudPoints;
using TimeLine;

namespace Core
{
	public class Class1
	{
		public Graph<HexNode, int> Grid { get; } = new Graph<HexNode, int>();

		public Axis TimeAxis { get; } = new Axis();
	}

	public class HexNode
	{
		public HexNode(Class1 owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Class1 Owner { get; }
	}

	public class Piece
	{
		
	}

	public class MovementPiece
	{
		public MovementPiece(HexNode current)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
		}

		public HexNode Current { get; private set; }

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
