using System;
using System.Drawing;
using System.Linq;
using StepFlow.Core;
using StepFlow.ViewModel.Marking;

namespace StepFlow.ViewModel
{
	public class NodeVm : ParticleVm<Node>, IMarkered, IParticleVm
	{
		public NodeVm(IServiceProvider serviceProvider) : base(serviceProvider)
		{
			// TODO Реализовать отписку или другой способ оповещения
			State.OnMarkChanged += StateOnMarkChanged;
		}

		private void StateOnMarkChanged(object sender, MarkChanged<NodeState> e)
		{
			OnPropertyChanged(nameof(State));
		}

		public Point Position => SourceRequired.Position;

		public bool IsOccupied => SourceRequired.Occupiers.Any();

		public MarkerCounter<NodeState> State { get; } = new MarkerCounter<NodeState>();

		private bool isMark;
		public bool IsMark
		{
			get => isMark;
			set => SetValue(ref isMark, value);
		}

		public PieceVm CreateSimple() => new PieceVm(ServiceProvider)
		{
			Current = this,
		};

		public override string? ToString() => Source?.ToString();
	}
}
