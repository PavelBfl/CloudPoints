using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Collections
{
	internal class PlaygroundUsedCollectionProxy<TItem, TList> : CollectionProxy<TItem, TList>
		where TItem : Material
		where TList : class, ICollection<TItem>
	{
		public PlaygroundUsedCollectionProxy(PlayMaster owner, TList target) : base(owner, target)
		{
		}

		public override void Add(TItem item)
		{
			base.Add(item);

			var proxy = (IPlaygroundUsed)Owner.CreateProxy(item);
			proxy.Begin();
		}

		public override bool Remove(TItem item)
		{
			var result = base.Remove(item);
			if (result)
			{
				var proxy = (IPlaygroundUsed)Owner.CreateProxy(item);
				proxy.End();
			}

			return result;
		}

		public override void Clear()
		{
			var items = this.ToArray();

			base.Clear();

			foreach (var item in items)
			{
				var proxy = (IPlaygroundUsed)Owner.CreateProxy(item);
				proxy.End();
			}
		}
	}
}
