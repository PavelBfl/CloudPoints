using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.GamePlay;
using StepFlow.ViewModel.Collections;

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

			foreach (var particle in Pieces)
			{
				particle.Refresh();
				particle.Commands.Refresh();
				particle.CommandsCompleted.Refresh();

				foreach (var command in particle.Commands)
				{
					command.Refresh();
				}
			}

			// TODO Реализовать общую реализацию через интерфейс
			foreach (var node in Place)
			{
				node.Refresh();
				node.Commands.Refresh();
				node.CommandsCompleted.Refresh();

				foreach (var command in node.Commands)
				{
					command.Refresh();
				}
			}
		}
	}

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

	public sealed class PlaceVm : WrapperObserver<NodeVm, Node>
	{
		public PlaceVm(ContextVm owner, IEnumerable<Node> items)
			: base(items)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private ContextVm Owner { get; }

		protected override NodeVm CreateObserver(Node observable)
		{
			if (Owner.WrapperProvider.TryGetViewModel(observable, out object result))
			{
				return (NodeVm)result;
			}
			else
			{
				return new NodeVm(Owner, observable);
			}
		}
	}
}
