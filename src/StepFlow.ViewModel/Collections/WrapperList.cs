using System.Collections.Generic;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperList<TWrapperItem, TCollection, TModelItem> : WrapperCollection<TWrapperItem, TCollection, TModelItem>, IReadOnlyList<TWrapperItem>
		where TCollection : class, IList<TModelItem>
		where TWrapperItem : class, IWrapper<TModelItem>
		where TModelItem : class
	{
		public WrapperList(LockProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public TWrapperItem this[int index] => LockProvider.GetOrCreate<TWrapperItem>(Source[index]);

		public int IndexOf(TWrapperItem item) => Source.IndexOf(item.Source);
	}
}
