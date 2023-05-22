using System.Collections.Generic;
using System.Linq;
using StepFlow.GamePlay;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<Context>
	{
		public ContextVm(int colsCount, int rowsCount)
			: base(new Context(colsCount, rowsCount))
		{
			Pieces = new PiecesCollectionVm(Source.World.Pieces);
			Place = new PlaceVm(Source.World.Place.Values);
			TimeAxis = new AxisVm(Source.AxisTime);
		}

		public PiecesCollectionVm Pieces { get; }

		public PlaceVm Place { get; }

		public IEnumerable<IParticleVm> Particles => Pieces.AsEnumerable<IParticleVm>().Concat(Place);

		public AxisVm TimeAxis { get; }

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

		public void TakeStep()
		{
			Source.World.TakeStep();

			Pieces.Refresh();
			Place.Refresh();
			TimeAxis.Refresh();

			foreach (var particle in Particles)
			{
				particle.Refresh();
			}
		}
	}
}
