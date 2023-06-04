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
		}

		private PiecesCollectionVm? pieces;

		public PiecesCollectionVm Pieces => pieces ??= LockProvider.GetOrCreate<PiecesCollectionVm>(Source.Pieces);

		private PlaceVm? place;

		public PlaceVm Place => place ??= LockProvider.GetOrCreate<PlaceVm>(Source.Place);

		public IEnumerable<IParticleVm> Particles => Pieces.AsEnumerable<IParticleVm>().Concat(Place);

		public void TakeStep()
		{
			Source.TakeStep();

			Pieces.SourceHasChange();

			if (Owner.Current is { } && !Pieces.Contains(Owner.Current))
			{
				Owner.Current = null;
			}

			Place.SourceHasChange();

			foreach (var particle in Particles)
			{
				particle.SourceHasChange();
			}

			Owner.TimeAxis.Refresh();

			LockProvider.Clear();
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(owner, pieces, place);
	}
}
