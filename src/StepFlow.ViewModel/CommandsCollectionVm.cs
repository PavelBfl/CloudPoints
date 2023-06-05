using System.Collections.Generic;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm<TTarget> : WrapperList<ICommandVm<ITargetingCommand<TTarget>>, IList<ITargetingCommand<TTarget>>, ITargetingCommand<TTarget>>
		where TTarget : notnull
	{
		public CommandsCollectionVm(LockProvider wrapperProvider, IList<ITargetingCommand<TTarget>> source) : base(wrapperProvider, source)
		{
		}
	}
}
