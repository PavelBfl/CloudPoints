using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.Common.Exceptions;

namespace StepFlow.ViewModel.Collections
{
	public abstract class EnumerableObserver<TObserver, TObservable> : IReadOnlyListObservable<TObserver>
	{
		public EnumerableObserver(IEnumerable<TObservable> items)
		{
			Items = items ?? throw new ArgumentNullException(nameof(items));
			NeedRefresh = true;
		}

		private bool RefreshObservers()
		{
			var hashChanges = false;
			if (NeedRefresh)
			{
				int index = 0;
				foreach (var item in Items)
				{
					if (index < observers.Count)
					{
						if (!ItemEquals(observers[index], item))
						{
							observers[index] = CreateObserver(item);
							hashChanges = true;
						}
					}
					else if (index == observers.Count)
					{
						observers.Add(CreateObserver(item));
						hashChanges = true;
					}
					index++;
				}

				while (index < observers.Count)
				{
					var removedIndex = observers.Count - 1;
					RemoverObserver(observers[removedIndex]);
					observers.RemoveAt(removedIndex);
					hashChanges = true;
				}

				NeedRefresh = false;
			}

			return hashChanges;
		}

		private List<TObserver> observers = new List<TObserver>();

		private List<TObserver> Observers
		{
			get
			{
				RefreshObservers();
				return observers;
			}
		}

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		private void ObCollectionChanged() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

		public IEnumerable<TObservable> Items { get; }

		private bool NeedRefresh { get; set; }

		public int Count => Observers.Count;

		public TObserver this[int index] => Observers[index];

		public void Refresh(RefreshState state = RefreshState.Lazy)
		{
			NeedRefresh = true;
			switch (state)
			{
				case RefreshState.Now:
					if (RefreshObservers())
					{
						ObCollectionChanged();
					}
					break;
				case RefreshState.Lazy:
					ObCollectionChanged();
					break;
				default: throw EnumNotSupportedException.Create(state);
			}
		}

		public IEnumerator<TObserver> GetEnumerator() => Observers.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		protected abstract bool ItemEquals(TObserver observer, TObservable observable);

		protected abstract TObserver CreateObserver(TObservable observable);

		protected abstract void RemoverObserver(TObserver observer);
	}
}
