using System.Collections;
using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Collections
{
	internal class CollectionProxy<TItem, TCollection> : ProxyBase<TCollection>, ICollection<TItem>
		where TCollection : class, ICollection<TItem>
	{
		public CollectionProxy(PlayMaster owner, TCollection target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => Target.IsReadOnly;

		public virtual void Add(TItem item) => Owner.TimeAxis.Add(new AddItemCommand<TItem>(Target, item));

		public virtual void Clear() => Owner.TimeAxis.Add(new ClearCommand<TItem>(Target));

		public bool Contains(TItem item) => Target.Contains(item);

		public void CopyTo(TItem[] array, int arrayIndex) => Target.CopyTo(array, arrayIndex);

		public IEnumerator<TItem> GetEnumerator() => Target.GetEnumerator();

		public virtual bool Remove(TItem item)
		{
			var removed = Target.Remove(item);
			if (removed)
			{
				Owner.TimeAxis.Add(new Reverse(new AddItemCommand<TItem>(Target, item)), true);
			}

			return removed;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
