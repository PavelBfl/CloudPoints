using System.Collections.Generic;
using System.Linq;

namespace StepFlow.ViewModel
{
	public class WorldVm : WrapperVm<GamePlay.World>
	{
		internal WorldVm(WrapperProvider wrapperProvider, GamePlay.World source) : base(wrapperProvider, source)
		{
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= WrapperProvider.GetOrCreate<ContextVm>(Source.Owner);

		private PiecesCollectionVm? pieces;

		public PiecesCollectionVm Pieces => pieces ??= WrapperProvider.GetOrCreate<PiecesCollectionVm>(Source.Pieces);

		private PlaceVm? place;

		public PlaceVm Place => place ??= WrapperProvider.GetOrCreate<PlaceVm>(Source.Place);

		public IEnumerable<IParticleVm> Particles => Pieces.AsEnumerable<IParticleVm>().Concat(Place);

		public void TakeStep() => Source.TakeStep();
	}
}
