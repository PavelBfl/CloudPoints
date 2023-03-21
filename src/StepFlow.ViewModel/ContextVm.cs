using System;
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
			Particles = new ParticlesCollectionVm(this);
			TimeAxis = new AxisVm(this, Source.AxisTime);
		}

		public ParticlesCollectionVm Particles { get; }

		public IEnumerable<PieceVm> Pieces => Particles.OfType<PieceVm>();

		public IEnumerable<NodeVm> Nodes => Particles.OfType<NodeVm>();

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
			Particles.Refresh();
		}
	}
}
