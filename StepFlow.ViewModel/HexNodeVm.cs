using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class HexNodeVm : WrapperVm<HexNode>
	{

		public HexNodeVm(HexNode source) : base(source, true)
		{
		}

		public bool IsSelected { get; set; }

		private NodeState state = NodeState.Node;
		public NodeState State
		{
			get => state;
			set => SetValue(ref state, value);
		}
	}

	public enum NodeState
	{
		Node,
		Current,
		Planned,
	}
}
