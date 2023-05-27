using System.Collections.Generic;
using System.Collections.Specialized;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperList<TWrapper, TCollection, TModelItem> : WrapperCollection<TWrapper, TCollection, TModelItem>, IList<TWrapper>, IReadOnlyList<TWrapper>
		where TCollection : IList<TModelItem>
		where TWrapper : WrapperVm<TModelItem>
		where TModelItem : notnull
	{
		public WrapperList(WrapperProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public TWrapper this[int index]
		{
			get => WrapperProvider.GetOrCreate<TWrapper>(Source[index]);
			set
			{
				var oldItem = this[index];
				if (!EqualityComparer<TModelItem>.Default.Equals(oldItem.Source, value.Source))
				{
					Source[index] = value.Source;
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
				}
			}
		}

		public override bool Remove(TWrapper item)
		{
			var index = IndexOf(item);
			var canRemove = index >= 0;
			if (canRemove)
			{
				RemoveAt(index);
			}

			return canRemove;
		}

		public int IndexOf(TWrapper item) => Source.IndexOf(item.Source);

		public void Insert(int index, TWrapper item)
		{
			Source.Insert(index, item.Source);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		public void RemoveAt(int index)
		{
			var oldItem = this[index];
			Source.RemoveAt(index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
		}
	}
}
