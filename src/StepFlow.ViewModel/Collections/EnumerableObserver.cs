using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.Common.Exceptions;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperEnumerable<TWrapper, TCollection, TModelItem> : WrapperVm<TCollection>, IEnumerable<TWrapper>, INotifyCollectionChanged
		where TCollection : IEnumerable<TModelItem>
	{
		public WrapperEnumerable(WrapperProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged.Invoke(this, args);

		public IEnumerator<TWrapper> GetEnumerator()
		{
			foreach (var model in Source)
			{
				yield return WrapperProvider.GetOrCreate<TWrapper>(model);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override void Refresh()
		{
			base.Refresh();

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}

	public class WrapperCollection<TWrapper, TCollection, TModelItem> : WrapperEnumerable<TWrapper, TCollection, TModelItem>, ICollection<TWrapper>
		where TCollection : ICollection<TModelItem>
		where TWrapper : WrapperVm<TModelItem>
	{
		public int Count => Source.Count;

		public bool IsReadOnly => false;

		public void Add(TWrapper item)
		{
			Source.Add(item.Source);

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public void Clear()
		{
			Source.Clear();

			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(TWrapper item) => Source.Contains(item.Source);

		public void CopyTo(TWrapper[] array, int arrayIndex) => this.ToArray().CopyTo(array, arrayIndex);

		public bool Remove(TWrapper item)
		{
			var result = Source.Remove(item.Source);
			if (result)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}

			return result;
		}
	}

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

		protected virtual void RemoverObserver(TObserver observer)
		{
		}
	}
}
