using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PiecesCollectionVm : CollectionWrapperObserver<PieceVm, Piece>
	{
		public PiecesCollectionVm(WrapperProvider wrapperProvider, ICollection<Piece> items)
			: base(items)
		{
			WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
		}

		private WrapperProvider WrapperProvider { get; }

		protected override PieceVm CreateObserver(Piece observable) => WrapperProvider.GetOrCreate<PieceVm>(observable);
	}
}
