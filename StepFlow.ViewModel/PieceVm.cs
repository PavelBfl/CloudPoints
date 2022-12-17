using System;
using StepFlow.Core;
using StepFlow.Core.Preset;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class PieceVm<T> : WrapperVm<T>, IPieceVm
		where T : Piece
	{
		public PieceVm(WorldVm world, T source, HexNodeVm current) : base(source, true)
		{
			Owner = world ?? throw new ArgumentNullException(nameof(world));
			Current = current ?? throw new ArgumentNullException(nameof(current));
			CommandQueue = new CommandsQueueVm(Source);
		}

		public WorldVm Owner { get; }

		private HexNodeVm current;
		public HexNodeVm Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					Current?.SetNode();

					current = value;

					Current.SetCurrent();
				}
			}
		}

		public CommandsQueueVm CommandQueue { get; }

		public void Add(ICommand command) => CommandQueue.Add(command);

		public void MoveTo(HexNodeVm node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			Add(new MoveCommand(Source, node.Source));
		}
	}
}
