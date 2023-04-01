using System;
using System.Collections.Generic;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

namespace StepFlow.ViewModel
{
	public sealed class PiecesCollectionVm : CollectionWrapperObserver<PieceVm, Piece>
	{
		public PiecesCollectionVm(ContextVm owner, ICollection<Piece> items)
			: base(items)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private ContextVm Owner { get; }

		protected override PieceVm CreateObserver(Piece observable)
		{
			if (Owner.WrapperProvider.TryGetViewModel(observable, out object result))
			{
				return (PieceVm)result;
			}
			else
			{
				return new PieceVm(Owner, observable);
			}
		}
	}
}
