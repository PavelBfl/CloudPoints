using System;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel.Commands
{
	public class CreateCommand : CommandBase
	{
		public CreateCommand(PieceVm current, PieceVm newPiece, NodeVm startNode)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
			NewPiece = newPiece ?? throw new ArgumentNullException(nameof(newPiece));
			StartNode = startNode ?? throw new ArgumentNullException(nameof(startNode));
		}

		public PieceVm Current { get; }

		public PieceVm NewPiece { get; }

		public NodeVm StartNode { get; }

		public bool IsMark { get; set; }

		public override void Execute()
		{
			NewPiece.Next = StartNode;
		}
	}
}
