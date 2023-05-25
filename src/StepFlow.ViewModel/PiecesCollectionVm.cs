using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PiecesCollectionVm : WrapperCollection<PieceVm, ICollection<Piece>, Piece>
	{
		public PiecesCollectionVm(WrapperProvider wrapperProvider, ICollection<Piece> items) : base(wrapperProvider, items)
		{
		}
	}
}
