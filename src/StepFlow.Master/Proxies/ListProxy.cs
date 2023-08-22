using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies
{
	public class ListProxy<TItem, TList> : CollectionProxy<TItem, TList>, IList<TItem>
		where TList : class, IList<TItem>
	{
		public ListProxy(PlayMaster owner, TList target) : base(owner, target)
		{
		}

		public TItem this[int index] { get => Target[index]; set => Owner.TimeAxis.Add(new SetItemCommand<TItem>(Target, value, index)); }

		public int IndexOf(TItem item) => Target.IndexOf(item);

		public void Insert(int index, TItem item) => Owner.TimeAxis.Add(new InsertItemCommand<TItem>(Target, item, index));

		public void RemoveAt(int index)
		{
			var oldItem = Target[index];
			Owner.TimeAxis.Add(new Reverse(new InsertItemCommand<TItem>(Target, oldItem, index)));
		}
	}
}
