using System;
using StepFlow.Core.Commands;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
    public class AxisVm : WrapperVm<Axis<ITargetingCommand<object>>>
	{
		public AxisVm(LockProvider wrapperProvider, Axis<ITargetingCommand<object>> source)
			: base(wrapperProvider, source)
		{
		}

		public void Add(ICommandVm<ITargetingCommand<object>> command, bool isCompleted = false)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			Source.Add(command.Source, isCompleted);
		}

		public bool? IsCompleted(ICommandVm<ITargetingCommand<object>> command)
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
