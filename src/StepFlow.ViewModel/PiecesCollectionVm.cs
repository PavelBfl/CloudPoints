using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
    public sealed class PiecesCollectionVm : WrapperCollection<PieceVm, ICollection<Piece>, Piece>
	{
		public PiecesCollectionVm(LockProvider wrapperProvider, ICollection<Piece> items) : base(wrapperProvider, items)
		{
		}
	}
}
