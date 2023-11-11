using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Collections
{
	internal class ListItemsProxy<TItem, TList, TItemProxy> : CollectionItemsProxy<TItem, TList, TItemProxy>, IList<TItemProxy>
		where TItemProxy : IReadOnlyProxyBase<TItem>
		where TList : class, IList<TItem>
	{
		public ListItemsProxy(PlayMaster owner, TList target) : base(owner, target)
		{
		}

		public TItemProxy this[int index]
		{
			get => (TItemProxy)Owner.CreateProxy(Target[index]);
			set => Owner.TimeAxis.Add(new SetItemCommand<TItem>(Target, value.Target, index));
		}

		public int IndexOf(TItemProxy item) => Target.IndexOf(item.Target);

		public void Insert(int index, TItemProxy item) => Owner.TimeAxis.Add(new InsertItemCommand<TItem>(Target, item.Target, index));

		public void RemoveAt(int index)
		{
			var oldItem = Target[index];
			Owner.TimeAxis.Add(new Reverse(new InsertItemCommand<TItem>(Target, oldItem, index)));
		}
	}
}
