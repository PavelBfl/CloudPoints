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

			stateToken = Node.State.Registry(NodeState.Planned);
		}

		public IPieceVm Current { get; }

		public HexNodeVm Node { get; }

		public bool IsMark
		{
			get => Node.IsMark;
			set => Node.IsMark = value;
		}

		private IDisposable? stateToken;

		public override void Execute()
		{
			Current.Current = Node;
		}

		public override void Dispose()
		{
			stateToken?.Dispose();
			stateToken = null;
			base.Dispose();
		}
	}
}
