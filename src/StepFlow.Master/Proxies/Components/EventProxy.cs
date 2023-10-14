using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Master.Commands.Collections;

namespace StepFlow.Master.Proxies.Components
{
	internal class EventProxy : ProxyBase<ICollection<Handler>>, ICollection<IHandlerProxy>
	{
		public EventProxy(PlayMaster owner, ICollection<Handler> target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => false;

		public void Add(IHandlerProxy item)
			=> Owner.TimeAxis.Add(new AddItemCommand<Handler>(Target, (Handler)item.Target));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<Handler>(Target));

		public bool Contains(IHandlerProxy item) => Target.Contains((Handler)item.Target);

		public void CopyTo(IHandlerProxy[] array, int arrayIndex)
		{
			foreach (var component in this)
			{
				array[arrayIndex] = component;
				arrayIndex++;
			}
		}

		public IEnumerator<IHandlerProxy> GetEnumerator() => Target.Select(x => (IHandlerProxy)Owner.CreateProxy(x)).GetEnumerator();

		public bool Remove(IHandlerProxy item) => Target.Remove((Handler)item.Target);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
