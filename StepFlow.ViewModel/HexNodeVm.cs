using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class HexNodeVm : WrapperVm<HexNode>
	{
		public HexNodeVm(HexNode source) : base(source, true)
		{
		}

		public bool IsSelected { get; set; }
	}
}
