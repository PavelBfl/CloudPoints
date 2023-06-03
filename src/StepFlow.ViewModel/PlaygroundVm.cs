using System.Collections.Generic;
using System.Linq;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class PlaygroundVm : WrapperVm<Playground>
	{
		internal PlaygroundVm(WrapperProvider wrapperProvider, Playground source) : base(wrapperProvider, source)
		{
		}

		private PiecesCollectionVm? pieces;

		public PiecesCollectionVm Pieces => pieces ??= WrapperProvider.GetOrCreate<PiecesCollectionVm>(Source.Pieces);

		private PlaceVm? place;

		public PlaceVm Place => place ??= WrapperProvider.GetOrCreate<PlaceVm>(Source.Place);

		public IEnumerable<IParticleVm> Particles => Pieces.AsEnumerable<IParticleVm>().Concat(Place);

		public void TakeStep()
		{
			Source.TakeStep();

			Pieces.Refresh();

			if (Owner.Current is { } && !Pieces.Contains(Owner.Current))
			{
				Owner.Current = null;
			}

			Place.Refresh();

			foreach (var particle in Particles)
			{
				particle.Refresh();
			}

			Owner.TimeAxis.Refresh();

			WrapperProvider.Clear();
		}

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(owner, pieces, place);
	}
}
