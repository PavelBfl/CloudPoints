using System;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class HexNodeVm : WrapperVm<HexNode>
	{
		public HexNodeVm(WorldVm owner, HexNode source) : base(source, true)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			// TODO Реализовать отписку или другой способ оповещения
			State.OnMarkChanged += StateOnMarkChanged;
		}

		private void StateOnMarkChanged(object sender, MarkChanged<NodeState> e)
		{
			OnPropertyChanged(nameof(State));
		}

		public WorldVm Owner { get; }

		public Point Position => new Point(Source.Col, Source.Row);

		public bool IsOccupied => Source.Occupiers.Any();

		public MarkerCounter<NodeState> State { get; } = new MarkerCounter<NodeState>();

		public PieceVm<Piece> CreateSimple() => new PieceVm<Piece>(
			Owner,
			new Piece(Owner.Source)
			{
				Current = Source,
			}
		)
		{
			Current = this,
		};

		public override string ToString() => Source.ToString();
	}
}
