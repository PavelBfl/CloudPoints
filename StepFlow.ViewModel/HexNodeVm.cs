using System;
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

		public bool IsSelected { get; set; }

		public void SetNode() => State = NodeState.Node;

		public void SetCurrent() => State = NodeState.Current;

		public void SetPlanned()
		{
			if (Owner.Current is { } current)
			{
				current.MoveTo(this);
				State = NodeState.Planned;
			}
		}

		private NodeState state = NodeState.Node;
		public NodeState State
		{
			get => state;
			private set => SetValue(ref state, value);
		}
	}

	public enum NodeState
	{
		Node,
		Current,
		Planned,
	}
}
