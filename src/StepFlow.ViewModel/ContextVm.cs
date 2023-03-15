using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Entities;
using StepFlow.GamePlay;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<Context>
	{
		public ContextVm(IServiceProvider serviceProvider, int colsCount, int rowsCount)
			: base(serviceProvider, new Context(colsCount, rowsCount))
		{
			Particles = new ParticlesCollectionVm(this);

			foreach (GamePlay.Node node in Source.World.Place.Values)
			{
				new NodeVm(serviceProvider, this, node);
			}

			TimeAxis = new AxisVm(ServiceProvider, Source.AxisTime);
		}

		public ParticlesCollectionVm Particles { get; }

		public IEnumerable<PieceVm> Pieces => Particles.ViewsModels.OfType<PieceVm>();

		public IEnumerable<NodeVm> Nodes => Particles.ViewsModels.OfType<NodeVm>();

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

			foreach (var particle in Particles.Models.Union(Source.World.Particles))
			{
				var containsVm = Particles.Contains(particle);
				var containsM = Source.World.Particles.Contains(particle);

				if (containsVm && !containsM)
				{
					Particles[particle].Dispose();
				}
				else if (!containsVm && containsM)
				{
					switch (particle)
					{
						case Core.Piece piece:
							new PieceVm(ServiceProvider, this, piece);
							break;
						case Node node:
							new NodeVm(ServiceProvider, this, node);
							break;
					}
				}
				else if (containsVm && containsM)
				{
					Particles[particle].TakeStep();
				}
			}
		}

	}
}
