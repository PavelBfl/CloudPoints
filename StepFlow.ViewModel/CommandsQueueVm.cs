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

		public void Add(ICommand command)
		{
			Source.Add(new LocalCommand(this, command)
			{
				IsSelected = IsSelected,
			});
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, command));
		}

		public IEnumerator<ICommandVm> GetEnumerator() => Source.CommandsQueue.Cast<ICommandVm>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class LocalCommand : CommandWrapper, ICommandVm
		{
			public LocalCommand(CommandsQueueVm owner, ICommand source)
				: base(source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CommandsQueueVm Owner { get; }

			public bool IsSelected { get; set; }

			public override void Dispose()
			{
				base.Dispose();
				Owner.CollectionChanged?.Invoke(Owner, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Source));
			}
		}
	}
}
