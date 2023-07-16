using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public sealed class PlaceVm : WrapperVm<Place>, IReadOnlyCollection<NodeVm>, INotifyCollectionChanged
	{
		internal PlaceVm(LockProvider wrapperProvider, Place items)
			: base(wrapperProvider, items)
		{
		}

		public int Count => Source.Count;

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		public bool Contains(NodeVm item) => Source.Values.Contains(item.Source);

		public void CopyTo(NodeVm[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		public IEnumerator<NodeVm> GetEnumerator() => Source.Values.Select(x => LockProvider.GetOrCreate<NodeVm>(x)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
