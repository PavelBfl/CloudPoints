using System.Collections.Generic;
using System.Linq;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel.Collections
{
	public class WrapperCollection<TWrapperItem, TCollection, TModelItem> : WrapperEnumerable<TWrapperItem, TCollection, TModelItem>, IReadOnlyCollection<TWrapperItem>
		where TWrapperItem : class, IWrapper<TModelItem>
		where TCollection : class, IReadOnlyCollection<TModelItem>
		where TModelItem : class
	{
		public WrapperCollection(LockProvider wrapperProvider, TCollection source) : base(wrapperProvider, source)
		{
		}

		public int Count => Source.Count;

		public bool Contains(TWrapperItem item) => Source.Contains(item.Source);

		public void CopyTo(TWrapperItem[] array, int arrayIndex) => this.ToArray().CopyTo(array, arrayIndex);
	}
}
