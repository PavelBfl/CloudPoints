using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Commands;
using StepFlow.Core.Commands.Preset;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<Node>, IMarkered
	{
		public NodeVm(LockProvider wrapperProvider, Node source)
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
				LockProvider,
				new CreateCommand(Source.Owner)
			);

			Owner.AxisTime.Add(command);

			Owner.Current = (PieceVm?)Owner.Buffer;

			Owner.Pieces.SourceHasChange();
			foreach (var piece in Owner.Pieces)
			{
				piece.SourceHasChange();
			}
		}

		public override string? ToString() => Source?.ToString();
	}
}
