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

		private AxisVm? axisTime;

		public AxisVm AxisTime => axisTime ??= LockProvider.GetOrCreate<AxisVm>(Source.AxisTime);

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

		public ILockable? Buffer => LockProvider.Get(Source.Buffer);

		public void TakeStep()
		{
			Source.TakeStep();

			Pieces.SourceHasChange();

			if (Current is { } && !Pieces.Contains(Current))
			{
				Current = null;
			}

			Place.SourceHasChange();

			foreach (var particle in Particles)
			{
				particle.SourceHasChange();
			}

			AxisTime.SourceHasChange();

			LockProvider.Clear();
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(pieces, place, axisTime, current);
	}
}
