using System;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
    public class AxisVm : WrapperVm<Axis<ICommand>>
	{
		public AxisVm(LockProvider wrapperProvider, Axis<ICommand> source)
			: base(wrapperProvider, source)
		{
		}

		public void Add(ICommandVm<ICommand> command, bool isCompleted = false)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			Source.Add(command.Source, isCompleted);
		}

		public bool? IsCompleted(ICommandVm<ICommand> command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			return Source.IsCompleted(command.Source);
		}

		public override void SourceHasChange()
		{
			foreach (var command in Source)
			{
				if (LockProvider.TryGetValue<IWrapper<object>>(command, out var commandVm))
				{
					commandVm.SourceHasChange();
				}
			}
		}
	}
}
