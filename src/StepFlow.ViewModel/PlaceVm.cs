using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperVm<Place<GamePlay.Node>>, ICollection<NodeVm>, INotifyCollectionChanged
	{
		internal PlaceVm(WrapperProvider wrapperProvider, Place<GamePlay.Node> items)
			: base(wrapperProvider, items)
		{
		}

		public int Count => Source.Count;

		public bool IsReadOnly => false;

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public void Add(NodeVm item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			Source.Add(item.Source);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
		}

		public void Clear()
		{
			Source.Clear();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(NodeVm item) => Source.Values.Contains(item.Source);

		public void CopyTo(NodeVm[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		public IEnumerator<NodeVm> GetEnumerator() => Source.Values.Select(x => WrapperProvider.GetOrCreate<NodeVm>(x)).GetEnumerator();

		public bool Remove(NodeVm item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			var removed = Source.Remove(item.Position);
			if (removed)
			{
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}

			return removed;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
