using System.Collections.Generic;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm<TCommand, TCommandVm, TTarget, TTargetVm> : WrapperList<TCommandVm, IList<TCommand>, TCommand>
		where TCommand : ITargetingCommand<TTarget>
		where TCommandVm : ICommandVm<TCommand>
		where TTarget : notnull
		where TTargetVm : IWrapper<TTarget>
	{
		public CommandsCollectionVm(LockProvider wrapperProvider, IList<TCommand> source) : base(wrapperProvider, source)
		{
		}
	}
}
