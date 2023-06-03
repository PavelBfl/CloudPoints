using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Commands;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<ITargetingCommand<Node>, Node>, IMarkered
	{
		public NodeVm(WrapperProvider wrapperProvider, Node source)
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

		public void CreateSimple()
		{
			var command = new CreateCommandVm(
				WrapperProvider,
				new CreateCommand(
					Source.Owner,
					new Strength(100),
					100,
					Source
				)
			);

			Owner.Owner.TimeAxis.Add(command);

			Owner.Owner.Current = command.Piece;

			Owner.Pieces.Refresh();
			foreach (var piece in Owner.Pieces)
			{
				piece.Refresh();
			}
		}

		public override string? ToString() => Source?.ToString();
	}
}
