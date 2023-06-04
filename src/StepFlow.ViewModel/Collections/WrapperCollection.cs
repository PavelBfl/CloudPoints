using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperCollection<TWrapper, TCollection, TModelItem> : WrapperEnumerable<TWrapper, TCollection, TModelItem>, ICollection<TWrapper>, IReadOnlyCollection<TWrapper>
		where TWrapper : IWrapper<TModelItem>
		where TCollection : ICollection<TModelItem>
		where TModelItem : notnull
	{
		public WrapperCollection(LockProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public int Count => Source.Count;

		public bool IsReadOnly => false;

		public virtual void Add(TWrapper item)
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

		public virtual bool Remove(TWrapper item)
		{
			var result = Source.Remove(item.Source);
			if (result)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}

			return result;
		}
	}
}
