using System;
using System.Collections.Generic;

namespace StepFlow.ViewModel.Collections
{
	public abstract class ListWrapperObserver<TWrapper, TSource> : CollectionWrapperObserver<TWrapper, TSource>, IListObservable<TWrapper>
		where TWrapper : WrapperVm<TSource>
		where TSource : notnull
	{
		protected ListWrapperObserver(IList<TSource> items)
			: base(items)
		{
			Items = items ?? throw new ArgumentNullException(nameof(items));
		}

		public new TWrapper this[int index]
		{
			get => base[index];
			set
			{
				Items.Add(value.Source);
				Refresh();
			}
		}

		public new IList<TSource> Items { get; }

		public int IndexOf(TWrapper item)
		{
			for (var i = 0; i < Count; i++)
			{
				if (EqualityComparer<TWrapper>.Default.Equals(this[i], item))
				{
					return i;
				}
			}

			return -1;
		}

		public void Insert(int index, TWrapper item)
		{
			Items.Insert(index, item.Source);
			Refresh();
		}

		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
			Refresh();
		}
	}
}
