using System.Collections.Generic;
using System;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class MovementPieceVm : WrapperVm<MovementPiece>
	{
		public MovementPieceVm(MovementPiece source) : base(source, true)
		{
		}

		public void MoveTo(HexNodeVm node)
		{
			
		}

		private class MoveCommand : CommandBase
		{
			public MoveCommand(MovementPiece owner, HexNode nextNode, ICollection<MoveCommand> container)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));
				Container = container ?? throw new ArgumentNullException(nameof(container));

				Container.Add(this);
			}

			public MovementPiece Owner { get; }

			public HexNode NextNode { get; }

			public ICollection<MoveCommand> Container { get; }

			public override void Execute()
			{
				Owner.Current = NextNode;
			}

			public override void Dispose()
			{
				Container.Remove(this);
			}
		}
	}
}
