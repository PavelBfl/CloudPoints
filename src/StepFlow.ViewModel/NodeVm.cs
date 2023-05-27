using System;
using System.Drawing;
using System.Linq;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<GamePlay.Node>, IMarkered
	{
		public NodeVm(WrapperProvider wrapperProvider, GamePlay.Node source)
			: base(wrapperProvider, source)
		{
			State.OnMarkChanged += StateOnMarkChanged;
		}

		private void StateOnMarkChanged(object sender, MarkChanged<NodeState> e)
		{
			OnPropertyChanged(nameof(State));
		}

		public Point Position => Source.Position;

		public bool IsOccupied => Source.Occupiers.Any();

		public MarkerCounter<NodeState> State { get; } = new MarkerCounter<NodeState>();

		public PieceVm CreateSimple()
		{
			var piece = new GamePlay.Piece(10)
			{
				CollisionDamage = 2,
			};

			var pieceVm = WrapperProvider.GetOrCreate<PieceVm>(piece);
			Owner.Pieces.Add(pieceVm);

			Owner.Owner.Current = pieceVm;
			return pieceVm;
		}

		public override string? ToString() => Source?.ToString();
	}
}
