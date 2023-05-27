using System;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class PieceVm : ParticleVm<GamePlay.Piece>, IMarkered
	{
		internal PieceVm(WrapperProvider wrapperProvider, GamePlay.Piece source)
			: base(wrapperProvider, source)
		{
		}

		private bool isMark = false;
		public bool IsMark
		{
			get => isMark;
			set => SetValue(ref isMark, value);
		}

		private IDisposable? stateToken;

		private NodeVm? current;
		public NodeVm? Current
		{
			get => current;
			set
			{
				if (Current != value)
				{
					stateToken?.Dispose();

					current = value;
					Source.Current = Current?.Source;

					stateToken = Current?.State.Add(NodeState.Current);
				}
			}
		}

		private NodeVm? next;
		public NodeVm? Next
		{
			get => next;
			set
			{
				if (Next != value)
				{
					next = value;
					Source.Next = Next?.Source;
				}
			}
		}

		public override void Refresh()
		{
			base.Refresh();

			Current = WrapperProvider.Get<NodeVm?>(Source.Current);
			Next = WrapperProvider.Get<NodeVm?>(Source.Next);
		}

		public void Add(CommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			command.IsMark = IsMark;
			Commands.Add(command);
		}

		public void MoveTo(NodeVm node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			var command = WrapperProvider.GetOrCreate<MoveCommandVm>(new GamePlay.Commands.MoveCommand(Source, node.Source));
			Add(command);
		}
	}
}
