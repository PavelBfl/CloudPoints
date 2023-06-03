﻿using System.Collections.Generic;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<GamePlay.Context>
	{
		internal ContextVm(WrapperProvider wrapperProvider, GamePlay.Context source) : base(wrapperProvider, source)
		{
			Lock = true;
		}

		private PlaygroundVm? playground;

		public PlaygroundVm Playground => playground ??= WrapperProvider.GetOrCreate<PlaygroundVm>(Source.Playground);

		private AxisVm? timeAxis;

		public AxisVm TimeAxis => timeAxis ??= WrapperProvider.GetOrCreate<AxisVm>(Source.AxisTime);

		private CommandsCollectionVm? staticCommands;

		public CommandsCollectionVm StaticCommands => staticCommands ??= WrapperProvider.GetOrCreate<CommandsCollectionVm>(Source.StaticCommands);

		private PieceVm? current = null;

		public PieceVm? Current
		{
			get => current;
			set
			{
				if (!Equals(Current, value))
				{
					OnPropertyChanging();

					if (Current is { })
					{
						Current.IsMark = false;
					}

					current = value;

					if (Current is { })
					{
						Current.IsMark = true;
					}

					OnPropertyChanged();
				}
			}
		}

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(playground, timeAxis, current);
	}
}
