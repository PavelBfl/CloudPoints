using System;
using System.Drawing;
using System.Linq;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm, IMarkered
	{
		public NodeVm(ContextVm owner, GamePlay.Node source)
			: base(owner, source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			// TODO Реализовать отписку или другой способ оповещения
			State.OnMarkChanged += StateOnMarkChanged;
		}

		public new GamePlay.Node Source { get; }

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

		public PieceVm CreateSimple()
		{
			var result = new PieceVm(Owner, new GamePlay.Piece(Owner.Source, 10))
			{
				Current = this,
			};
			Owner.Particles.Refresh();
			return result;
		}

		public override string? ToString() => Source?.ToString();
	}
}
