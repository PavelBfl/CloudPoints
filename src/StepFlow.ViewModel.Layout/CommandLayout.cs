using StepFlow.Layout;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel.Layout
{
	public class CommandLayout : RectPlot
	{
		public ICommandVm<ICommand>? Command { get; set; }
	}
}
