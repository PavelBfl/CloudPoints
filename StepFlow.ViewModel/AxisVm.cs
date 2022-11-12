using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis>
	{
		public AxisVm(Axis source) : base(source, true)
		{
		}

		public bool MoveNext() => Source.MoveNext();
	}
}
