using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public class PlaygroundVm : WrapperVm<Playground>
	{
		internal PlaygroundVm(LockProvider wrapperProvider, Playground source) : base(wrapperProvider, source)
		{
			Lock = true;
		}

		private PiecesCollectionVm? pieces;

		public PiecesCollectionVm Pieces => pieces ??= LockProvider.GetOrCreate<PiecesCollectionVm>(Source.Pieces);

		private PlaceVm? place;

		public PlaceVm Place => place ??= LockProvider.GetOrCreate<PlaceVm>(Source.Place);

		public IEnumerable<IParticleVm> Particles => Pieces.AsEnumerable<IParticleVm>().Concat(Place);

		private PieceVm? current = null;

		public PieceVm? Current
		{
			get => current;
			set
			{
				if (!Equals(Current, value))
				{
					OnPropertyChanging();

					if (Current is { })
					{
						Current.IsMark = false;
					}

					current = value;

					if (Current is { })
					{
						Current.IsMark = true;
					}

					OnPropertyChanged();
				}
			}
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(pieces, place, current);
	}
}
