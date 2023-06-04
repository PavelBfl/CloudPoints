using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<Context>
	{
		internal ContextVm(LockProvider wrapperProvider, Context source) : base(wrapperProvider, source)
		{
			Lock = true;
		}

		private PlaygroundVm? playground;

		public PlaygroundVm Playground => playground ??= LockProvider.GetOrCreate<PlaygroundVm>(Source.Playground);

		private AxisVm? timeAxis;

		public AxisVm TimeAxis => timeAxis ??= LockProvider.GetOrCreate<AxisVm>(Source.AxisTime);

		private CommandsCollectionVm? staticCommands;

		public CommandsCollectionVm StaticCommands => staticCommands ??= LockProvider.GetOrCreate<CommandsCollectionVm>(Source.StaticCommands);

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

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(playground, timeAxis, current);
	}
}
