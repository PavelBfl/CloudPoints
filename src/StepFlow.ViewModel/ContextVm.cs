using System.Collections.Generic;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class ContextVm : WrapperVm<GamePlay.Context>
	{
		public static ContextVm Create() => WrapperProvider.GetOrCreate<ContextVm>(new GamePlay.Context());

		internal ContextVm(WrapperProvider wrapperProvider, GamePlay.Context source) : base(wrapperProvider, source)
		{
		}

		private WorldVm? world;

		public WorldVm World => world ??= WrapperProvider.GetOrCreate<WorldVm>(Source.World);

		private AxisVm? timeAxis;

		public AxisVm TimeAxis => timeAxis ??= WrapperProvider.GetOrCreate<AxisVm>(Source.AxisTime);

		private CommandsCollection? staticCommands;

		public CommandsCollection StaticCommands => staticCommands ??= WrapperProvider.GetOrCreate<CommandsCollection>(Source.StaticCommands);

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
	}

	public class CommandsCollection : WrapperList<CommandVm, IList<GamePlay.Commands.Command>, GamePlay.Commands.Command>
	{
		public CommandsCollection(WrapperProvider wrapperProvider, IList<GamePlay.Commands.Command> source) : base(wrapperProvider, source)
		{
		}
	}

	public class WorldVm : WrapperVm<GamePlay.World>
	{
		internal WorldVm(WrapperProvider wrapperProvider, GamePlay.World source) : base(wrapperProvider, source)
		{
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= WrapperProvider.GetOrCreate<ContextVm>(Source.Owner);

		private PiecesCollectionVm? pieces;

		public PiecesCollectionVm Pieces => pieces ??= WrapperProvider.GetOrCreate<PiecesCollectionVm>(Source.Pieces);

		private PlaceVm? place;

		public PlaceVm Place => place ??= WrapperProvider.GetOrCreate<PlaceVm>(Source.Place);

		public void TakeStep() => Source.TakeStep();
	}
}
