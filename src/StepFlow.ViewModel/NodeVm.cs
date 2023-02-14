using System;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Entities;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<Node>, IMarkered, IParticleVm
	{
		public NodeVm(WorldVm owner, Node source) : base(owner, source)
		{
			// TODO Реализовать отписку или другой способ оповещения
			State.OnMarkChanged += StateOnMarkChanged;
		}

		private void StateOnMarkChanged(object sender, MarkChanged<NodeState> e)
		{
			OnPropertyChanged(nameof(State));
		}

		public Point Position => Source.Position;

		public bool IsOccupied => Source.Occupiers.Any();

		public MarkerCounter<NodeState> State { get; } = new MarkerCounter<NodeState>();

		private bool isMark;
		public bool IsMark
		{
			get => isMark;
			set => SetValue(ref isMark, value);
		}

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
