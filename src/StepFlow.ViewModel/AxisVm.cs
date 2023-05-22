using System;
using System.Linq;
using StepFlow.GamePlay.Commands;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis<Command>>
	{
		public AxisVm(Axis<Command> source)
			: base(source)
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
				command.GetOrCreate<CommandVm>().Refresh();
			}
		}
	}
}
