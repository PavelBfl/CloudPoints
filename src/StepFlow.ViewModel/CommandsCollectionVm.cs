using System;
using System.Collections.Generic;
using StepFlow.GamePlay.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
    public class CommandsCollectionVm : ListWrapperObserver<CommandVm, Command>, IMarkered
	{
		public CommandsCollectionVm(WrapperProvider wrapperProvider, IList<Command> commands)
			: base(commands)
		{
			WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
		}

		private WrapperProvider WrapperProvider { get; }

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

		protected override CommandVm CreateObserver(Command observable)
			=> WrapperProvider.GetOrCreate<CommandVm>(observable);
	}
}
