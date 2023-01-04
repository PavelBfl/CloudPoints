using System;
using System.Linq;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class PieceVm<T> : WrapperVm<T>, IPieceVm
		where T : Piece
	{
		public PieceVm(WorldVm world, T source) : base(source, true)
		{
			Owner = world ?? throw new ArgumentNullException(nameof(world));
			CommandQueue = new CommandsQueueVm(this);

			Owner.Pieces.Add(this);
		}

		public WorldVm Owner { get; }

		private bool isSelected = false;
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (IsSelected != value)
				{
					isSelected = value;

					CommandQueue.IsSelected = IsSelected;

					if (Current is { })
					{
						Current.State = IsSelected ? NodeState.Current : NodeState.Node;
					}
				}
			}
		}

		private HexNodeVm? current;
		public HexNodeVm? Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					if (Current is { })
					{
						Current.State = NodeState.Node;
					}

					current = value;
					Source.Current = Current?.Source;

					if (Current is { })
					{
						Current.State = NodeState.Current;
					}
				}
			}
		}

		public CommandsQueueVm CommandQueue { get; }

		public void Add(ICommandVm command) => CommandQueue.Add(command);

		public void MoveTo(HexNodeVm node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}
		}
	}
}
