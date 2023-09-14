using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Master.Commands.Collections;

namespace StepFlow.Master.Proxies.Components
{
	internal class EventProxy : ProxyBase<ICollection<IComponentChild>>, ICollection<IComponentProxy>
	{
		public EventProxy(PlayMaster owner, ICollection<IComponentChild> target) : base(owner, target)
		{
		}

		public int Count => Target.Count;

		public bool IsReadOnly => false;

		private IComponentChild GetTargetComponent(IComponentProxy componentProxy)
		{
			if (componentProxy is IProxyBase<IComponentChild> componentTarget)
			{
				return componentTarget.Target;
			}
			else
			{
				return (IComponentChild)componentProxy;
			}
		}

		public void Add(IComponentProxy item)
			=> Owner.TimeAxis.Add(new AddItemCommand<IComponentChild>(Target, GetTargetComponent(item)));

		public void Clear() => Owner.TimeAxis.Add(new ClearCommand<IComponentChild>(Target));

		public bool Contains(IComponentProxy item) => Target.Contains(GetTargetComponent(item));

		public void CopyTo(IComponentProxy[] array, int arrayIndex)
		{
			foreach (var component in this)
			{
				array[arrayIndex] = component;
				arrayIndex++;
			}
		}

		public IEnumerator<IComponentProxy> GetEnumerator() => Target.Select(x => Owner.CreateComponentProxy(x)).GetEnumerator();

		public bool Remove(IComponentProxy item) => Target.Remove(GetTargetComponent(item));

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
