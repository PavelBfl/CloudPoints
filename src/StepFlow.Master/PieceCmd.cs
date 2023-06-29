using StepFlow.Core;
using StepFlow.Core.Commands.Accessors;

namespace StepFlow.Master
{
	internal class PieceCmd : ParticleCmd<Piece>, IPieceCmd
	{
		public PieceCmd(PlayMaster owner, Piece source) : base(owner, source)
		{
		}

		private INodeCmd? Create(Node? node) => node is { } ? new NodeCmd(Owner, node) : null;

		public INodeCmd? Current { get => Create(Source.Current); set => Source.Current = value?.Source; }

		public INodeCmd? Next { get => Create(Source.Next); set => Source.Next = value?.Source; }

		public bool IsScheduledStep { get => Source.IsScheduledStep; set => Owner.TimeAxis.Add(Source.CreatePropertyCommand(x => x.IsScheduledStep, value)); }
	}
}
