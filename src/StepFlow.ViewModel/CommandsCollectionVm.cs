using System.Collections.Generic;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class CommandsCollectionVm : WrapperList<CommandVm, IList<GamePlay.Commands.Command>, GamePlay.Commands.Command>, IMarkered
	{
		public CommandsCollectionVm(WrapperProvider wrapperProvider, IList<GamePlay.Commands.Command> source) : base(wrapperProvider, source)
		{
		}

		private bool isMark;

		public bool IsMark
		{
			get => isMark;
			set
			{
				if (IsMark != value)
				{
					isMark = value;

					foreach (var command in this)
					{
						command.IsMark = IsMark;
					}
				}
			}
		}
	}
}
