using System;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommand : CommandBase, ICommandVm
	{
		public MoveCommand(IPieceVm current, NodeVm node)
		{
			Current = current ?? throw new ArgumentNullException(nameof(current));
			Node = node ?? throw new ArgumentNullException(nameof(node));

			stateToken = Node.State.Registry(NodeState.Planned);
		}

		public IPieceVm Current { get; }

		public NodeVm Node { get; }

		public bool IsMark
		{
			get => Node.IsMark;
			set => Node.IsMark = value;
		}

		private IDisposable? stateToken;

		public override void Execute()
		{
			Current.Next = Node;
		}

		public override void Dispose()
		{
			stateToken?.Dispose();
			stateToken = null;
			base.Dispose();
		}
	}
}
