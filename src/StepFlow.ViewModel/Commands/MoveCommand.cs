using System;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommand : CommandVm
	{
		public MoveCommand(IServiceProvider serviceProvider, ContextVm owner, PieceVm target, NodeVm node)
			: base(serviceProvider, new GamePlay.MoveCommand(owner.Source, target.Source, node.Source))
		{
			Next = node;
		}

		public NodeVm Next { get; }

		public override bool IsMark
		{
			get => Next.IsMark;
			set => Next.IsMark = value;
		}

		public override void Execute()
		{
			((PieceVm)Target).Next = Next;
		}
	}
}
