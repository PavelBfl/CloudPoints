using System;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommand : CommandVm
	{
		public MoveCommand(WrapperProvider wrapperProvider, GamePlay.Commands.MoveCommand source)
			: base(wrapperProvider, source)
		{
			Source = source;

			Refresh();
		}

		private new GamePlay.Commands.MoveCommand Source { get; }

		private NodeVm? next;

		public NodeVm Next { get => next ??= (NodeVm)WrapperProvider.GetViewModel(Source.Next); }

		private IDisposable? StateToken { get; set; }

		public override bool IsMark { get => Next.IsMark; set => Next.IsMark = value; }

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
				StateToken ??= Next.State.Add(NodeState.Planned);
			}
		}
	}
}
