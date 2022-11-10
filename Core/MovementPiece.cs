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

		public HexNode Current { get; set; }

		public void Enqueue(ICommand command)
		{
			Add(Owner.TimeAxis.Current + CommandsQueue.Count + 1, command);
		}
	}
}
