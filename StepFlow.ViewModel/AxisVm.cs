using System;
using System.Net.Http.Headers;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class AxisVm : Axis<ICommandVm>
	{
		protected override ICommandVm RegistryHandle(ICommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			return command.Current?.CommandQueue.Registry(command) ?? command;
		}
	}
}
