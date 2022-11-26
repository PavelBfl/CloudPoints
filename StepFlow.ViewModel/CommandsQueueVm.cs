using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using StepFlow.Core;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : IReadOnlyList<ICommand>, INotifyCollectionChanged
	{
		public CommandsQueueVm(Piece source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public ICommand this[int index] => Source.CommandsQueue[index];

		public int Count => Source.CommandsQueue.Count;

		private Piece Source { get; }

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public void Add(ICommand command)
		{
			Source.Add(command);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, command));
		}

		public IEnumerator<ICommand> GetEnumerator() => Source.CommandsQueue.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class LocalCommand : CommandWrapper
		{
			public LocalCommand(CommandsQueueVm owner, ICommand source)
				: base(source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CommandsQueueVm Owner { get; }

			public override void Dispose()
			{
				base.Dispose();
				Owner.CollectionChanged?.Invoke(Owner, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Source));
			}
		}
	}
}
