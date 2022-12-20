using System;
using System.Drawing;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class HexNodeVm : WrapperVm<HexNode>
	{
		public HexNodeVm(WorldVm owner, HexNode source) : base(source, true)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public WorldVm Owner { get; }

		public Point Position => new Point(Source.Col, Source.Row);

		public bool IsSelected { get; set; }

		private NodeState state = NodeState.Node;
		public NodeState State
		{
			get => state;
			set => SetValue(ref state, value);
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
	}

	public enum NodeState
	{
		Node,
		Current,
		Planned,
	}
}
