using System.Collections.Generic;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm : WrapperList<CommandVm, IList<GamePlay.Commands.Command>, GamePlay.Commands.Command>
	{
		public CommandsCollectionVm(WrapperProvider wrapperProvider, IList<GamePlay.Commands.Command> source) : base(wrapperProvider, source)
		{
		}
	}
}
