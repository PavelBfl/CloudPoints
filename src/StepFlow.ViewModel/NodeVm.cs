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

			var result = WrapperProvider.GetOrCreate<PieceVm>(piece);
			result.Current = this;

			Owner.Pieces.Refresh();
			return result;
		}

		public override string? ToString() => Source?.ToString();
	}
}
