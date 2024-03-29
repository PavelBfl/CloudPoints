﻿using System.Collections.Generic;
using StepFlow.Master.Commands.Collections;
using StepFlow.TimeLine;

namespace StepFlow.Master.Proxies.Collections
{
	internal class ListProxy<TItem, TList> : CollectionProxy<TItem, TList>, IList<TItem>
		where TList : class, IList<TItem>
	{
		public ListProxy(PlayMaster owner, TList target) : base(owner, target)
		{
		}

		public TItem this[int index]
		{
			get => Target[index];
			set => Owner.TimeAxis.Add(new SetItemCommand<TItem>(Target, value, index));
		}

		public int IndexOf(TItem item) => Target.IndexOf(item);

		public void Insert(int index, TItem item) => Owner.TimeAxis.Add(new InsertItemCommand<TItem>(Target, item, index));

		public override bool Remove(TItem item)
		{
			var index = IndexOf(item);
			var removed = index >= 0;
			if (removed)
			{
				RemoveAt(index);
			}

			return removed;
		}

		public void RemoveAt(int index)
		{
			var item = Target[index];
			Owner.TimeAxis.Add(new Reverse(new InsertItemCommand<TItem>(Target, item, index)));
		}
	}
}
