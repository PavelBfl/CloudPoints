using System.Collections.Generic;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm<TCommand, TTarget, TTargetVm> : WrapperList<CommandVm<TCommand, TTarget, TTargetVm>, IList<TCommand>, TCommand>
		where TTarget : notnull
		where TCommand : ITargetingCommand<TTarget>
		where TTargetVm : WrapperVm<TTarget>
	{
		public CommandsCollectionVm(WrapperProvider wrapperProvider, IList<TCommand> source) : base(wrapperProvider, source)
		{
		}
	}
}
