using System;
using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands;
using StepFlow.ViewModel.Collector;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class PieceVm : ParticleVm<Piece>, IMarkered
	{
		internal PieceVm(LockProvider wrapperProvider, Piece source)
			: base(wrapperProvider, source)
		{
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

		private CommandsCollectionVm<Piece>? commands;

		public CommandsCollectionVm<Piece> Commands => commands ??= LockProvider.GetOrCreate<CommandsCollectionVm<Piece>>(Source.Commands);

		public override void SourceHasChange()
		{
			base.SourceHasChange();

			Current = LockProvider.Get<NodeVm?>(Source.Current);
			Next = LockProvider.Get<NodeVm?>(Source.Next);
		}

		public void MoveTo(NodeVm node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			var command = LockProvider.GetOrCreate<MoveCommandVm>(new MoveCommand(Source, node.Source));
			command.IsMark = IsMark;
			Commands.Add(command);
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(current, next);

		public override void Dispose()
		{
			stateToken?.Dispose();

			base.Dispose();
		}
	}
}
