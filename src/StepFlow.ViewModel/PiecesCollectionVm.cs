using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PiecesCollectionVm : CollectionWrapperObserver<PieceVm, Piece>
	{
		public PiecesCollectionVm(ICollection<Piece> items)
			: base(items)
		{
		}

		protected override PieceVm CreateObserver(Piece observable) => WrapperProvider.GetOrCreate<PieceVm>(observable);
	}
}
