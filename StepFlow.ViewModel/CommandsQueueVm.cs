using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : IReadOnlyList<ICommandVm>, INotifyCollectionChanged, ISelectable
	{
		public CommandsQueueVm(Piece source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public ICommandVm this[int index] => (ICommandVm)Source.CommandsQueue[index];

		public int Count => Source.CommandsQueue.Count;

		private bool isSelected;
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (IsSelected != value)
				{
					isSelected = value;

					foreach (var command in this)
					{
						command.IsSelected = IsSelected;
					}
				}
			}
		}

		private Piece Source { get; }

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public void Add(ICommandVm command)
		{
			if (command is null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			command.IsSelected = IsSelected;
			Source.Add(new LocalCommand(this, command)
			{
				IsSelected = IsSelected
			});
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, command));
		}

		public IEnumerator<ICommandVm> GetEnumerator() => Source.CommandsQueue.Select(x => (ICommandVm)x).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class LocalCommand : CommandBase, ICommandVm
		{
			public LocalCommand(CommandsQueueVm owner, ICommandVm source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public CommandsQueueVm Owner { get; }
			public ICommandVm Source { get; }

			public bool IsSelected { get => Source.IsSelected; set => Source.IsSelected = value; }

			public override void Dispose()
			{
				base.Dispose();
				Owner.CollectionChanged?.Invoke(Owner, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Source));
			}
		}
	}

	public class MoveCommandVm : CommandBase, ICommandVm
	{
		public MoveCommandVm(IPieceVm piece, HexNodeVm nextNode)
		{
			Piece = piece ?? throw new ArgumentNullException(nameof(piece));
			NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));

			Refresh();
		}

		public IPieceVm Piece { get; }
		public HexNodeVm NextNode { get; }

		private bool isSelected;
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (IsSelected != value)
				{
					isSelected = value;

					Refresh();
				}
			}
		}

		private void Refresh()
		{
			NextNode.State = IsSelected ? NodeState.Planned : NodeState.Node;
		}

		public override void Execute()
		{
			Piece.Current = NextNode;
		}
	}
}
