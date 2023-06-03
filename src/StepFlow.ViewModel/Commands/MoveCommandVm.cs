using System;
using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommandVm : CommandVm<MoveCommand, Piece, PieceVm>
	{
		public MoveCommandVm(WrapperProvider wrapperProvider, MoveCommand source)
			: base(wrapperProvider, source)
		{
			Source = source;

			Refresh();
		}

		private new MoveCommand Source { get; }

		private NodeVm? next;

		public NodeVm Next => next ??= WrapperProvider.GetOrCreate<NodeVm>(Source.Next);

		private IDisposable? StateToken { get; set; }

		public override bool IsMark { get => Next.IsMark; set => Next.IsMark = value; }

		public override void Refresh()
		{
			base.Refresh();

			if (Owner.TimeAxis.IsCompleted(this) ?? false)
			{
				StateToken?.Dispose();
				StateToken = null;
			}
			else
			{
				StateToken ??= Next.State.Add(NodeState.Planned);
			}
		}

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(next);
	}
}
