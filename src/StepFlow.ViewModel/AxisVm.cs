using System;
using StepFlow.GamePlay;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis<Command>>
	{
		public AxisVm(IServiceProvider serviceProvider, Axis<Command> source)
			: base(serviceProvider, source)
		{
		}
	}
}
