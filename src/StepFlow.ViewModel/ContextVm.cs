using System.Collections.Generic;
using System.Linq;
using StepFlow.GamePlay;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<Context>
	{
		public ContextVm(IContextElement context, int colsCount, int rowsCount)
			: base(context, new Context(colsCount, rowsCount))
		{
			Pieces = new PiecesCollectionVm(this, Source.World.Pieces);
			Place = new PlaceVm(this, Source.World.Place.Values);
			TimeAxis = new AxisVm(this, Source.AxisTime);
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

			foreach (var particle in Particles)
			{
				particle.Refresh();
			}
		}
	}
}
