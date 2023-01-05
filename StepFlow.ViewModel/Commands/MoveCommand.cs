using System;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommand : CommandBase, ICommandVm
	{
		public MoveCommand(IPieceVm current, HexNodeVm node)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
			Node = node ?? throw new ArgumentNullException(nameof(node));
		}

		public IPieceVm Current { get; }

		public HexNodeVm Node { get; }

		public bool IsMark
		{
			get => Current.IsMark;
			set
			{
				Current.IsMark = value;

				if (value)
				{
					Node.State = Node.State | NodeState.Planned;
				}
				else
				{
					Node.State = Node.State & ~NodeState.Planned;
				}
			}
		}

		public override void Execute()
		{
			Current.Current = Node;
		}
	}
}
