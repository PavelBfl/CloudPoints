using System;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommand : CommandVm
	{
		public MoveCommand(IContextElement context, PieceVm target, NodeVm next)
			: base(context, new GamePlay.MoveCommand(target.Source, next.Source))
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));

			Refresh();
		}

		public NodeVm Next { get; }

		private IDisposable? StateToken { get; set; }

		public override bool IsMark
		{
			get => Next.IsMark;
			set => Next.IsMark = value;
		}

		public override void Refresh()
		{
			base.Refresh();

			if (Source.IsCompleted)
			{
				StateToken?.Dispose();
				StateToken = null;
			}
			else
			{
				if (StateToken is null)
				{
					StateToken = Next.State.Registry(NodeState.Planned);
				}
			}
		}
	}
}
