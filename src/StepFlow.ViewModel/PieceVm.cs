using System;
using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public class PieceVm : ParticleVm<Piece>, IMarkered
	{
		internal PieceVm(LockProvider wrapperProvider, Piece source)
			: base(wrapperProvider, source)
		{
		}

		private IDisposable? stateToken;

		private NodeVm? current;

		public NodeVm? Current => LockProvider.Get<NodeVm?>(Source.Current);

		private NodeVm? next;

		public NodeVm? Next => LockProvider.Get<NodeVm?>(Source.Next);

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(current, next);

		public override void Dispose()
		{
			stateToken?.Dispose();

			base.Dispose();
		}
	}
}
