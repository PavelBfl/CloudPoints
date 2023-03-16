using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.ViewModel.Collections
{
	public abstract class CollectionWrapperObserver<TWrapper, TSource> : WrapperObserver<TWrapper, TSource>, ICollectionObservable<TWrapper>
		where TWrapper : WrapperVm<TSource>
		where TSource : notnull
	{
		public CollectionWrapperObserver(ICollection<TSource> items)
			: base(items)
		{
			Items = items ?? throw new ArgumentNullException(nameof(items));
		}

		private new ICollection<TSource> Items { get; }

		public bool IsReadOnly => false;

		public void Add(TWrapper item)
		{
			Items.Add(item.Source);
			Refresh();
		}

		public void Clear()
		{
			Items.Clear();
			Refresh();
		}

		public bool Contains(TWrapper item) => this.AsEnumerable().Contains(item);

		public void CopyTo(TWrapper[] array, int arrayIndex) => this.ToArray().CopyTo(array, arrayIndex);

		public bool Remove(TWrapper item)
		{
			var removed = Items.Remove(item.Source);

			if (removed)
			{
				Refresh();
			}

			return removed;
		}
	}
}
