using System;
using System.Drawing;
using System.Linq;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<GamePlay.Node>, IMarkered
	{
		public NodeVm(GamePlay.Node source)
			: base(source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
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
			var piece = new GamePlay.Piece(10)
			{
				CollisionDamage = 2,
			};
			Owner.Source.World.Pieces.Add(piece);

			var result = new PieceVm(piece)
			{
				Current = this,
			};
			Owner.Pieces.Refresh();
			return result;
		}

		public override string? ToString() => Source?.ToString();

		public override void Dispose()
		{
			base.Dispose();

			State.OnMarkChanged -= StateOnMarkChanged;
		}
	}
}
