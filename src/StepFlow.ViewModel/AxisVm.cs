using System;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class AxisVm : WrapperVm<Axis<ICommand>>
	{
		public AxisVm(WrapperProvider wrapperProvider, Axis<ICommand> source)
			: base(wrapperProvider, source)
		{
		}

		public void Add(ICommandVm<IWrapper> command, bool isCompleted = false)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			Source.Add(command.Source, isCompleted);
		}

		public bool? IsCompleted(CommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			return Source.IsCompleted(command.Source);
		}

		public override void Refresh()
		{
			foreach (var command in Source)
			{
				if (WrapperProvider.TryGetValue<CommandVm>(command, out var commandVm))
				{
					commandVm.Refresh();
				}
			}
		}
	}
}
