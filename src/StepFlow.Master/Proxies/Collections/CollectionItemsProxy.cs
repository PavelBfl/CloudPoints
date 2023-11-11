using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Collections
{
	internal class CollectionItemsProxy<TItem, TCollection, TItemProxy> : ProxyBase<TCollection>, ICollection<TItemProxy>
		where TCollection : class, ICollection<TItem>
		where TItemProxy : IReadOnlyProxyBase<TItem>
	{
		public CollectionItemsProxy(PlayMaster owner, TCollection target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => Target.IsReadOnly;

		public void Add(TItemProxy item) => Owner.TimeAxis.Add(new AddItemCommand<TItem>(Target, item.Target));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<TItem>(Target));

		public bool Contains(TItemProxy item) => Target.Contains(item.Target);

		public void CopyTo(TItemProxy[] array, int arrayIndex)
		{
			foreach (var item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		public IEnumerator<TItemProxy> GetEnumerator() => Target.Select(x => (TItemProxy)Owner.CreateProxy(x)).GetEnumerator();

		public bool Remove(TItemProxy item)
		{
			var removed = Target.Remove(item.Target);
			if (removed)
			{
				Owner.TimeAxis.Add(new Reverse(new AddItemCommand<TItem>(Target, item.Target)));
			}

			return removed;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
