using System;
using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Commands
{
	public class MoveCommandVm : CommandVm<MoveCommand, Piece, PieceVm>
	{
		public MoveCommandVm(LockProvider wrapperProvider, MoveCommand source)
			: base(wrapperProvider, source)
		{
			Source = source;

			SourceHasChange();
		}

		private new MoveCommand Source { get; }

		private NodeVm? next;

		public NodeVm Next => next ??= LockProvider.GetOrCreate<NodeVm>(Source.Next);

		private IDisposable? StateToken { get; set; }

		public override bool IsMark { get => Next.IsMark; set => Next.IsMark = value; }

		public override void SourceHasChange()
		{
			base.SourceHasChange();

			if (Target.Owner.AxisTime.IsCompleted(this) ?? false)
			{
				StateToken?.Dispose();
				StateToken = null;
			}
			else
			{
				StateToken ??= Next.State.Add(NodeState.Planned);
			}
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(next);
	}
}
