using System;
using System.Linq;
using StepFlow.GamePlay.Commands;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis<Command>>
	{
		public AxisVm(WrapperProvider wrapperProvider, Axis<Command> source)
			: base(wrapperProvider, source)
		{
		}

		public bool? IsCompleted(CommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			return Source.IsCompleted(command.Source);
		}

		public void Refresh()
		{
			foreach (var command in Source)
			{
				if (WrapperProvider.TryGetViewModel(command, out var viewModel))
				{
					((CommandVm)viewModel).Refresh();
				}
			}
		}
	}
}
