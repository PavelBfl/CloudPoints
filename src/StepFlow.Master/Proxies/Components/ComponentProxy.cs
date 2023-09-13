using System.Collections.Generic;
using System.ComponentModel;
using StepFlow.Master.Proxies.Collections;

namespace StepFlow.Master.Proxies.Components
{
	public class ComponentProxy<TComponent> : ProxyBase<TComponent>, IComponentProxy
		where TComponent : Component
	{
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Target.Container);

		protected ICollection<uint> CreateEvenProxy(ICollection<uint> @event) => new CollectionProxy<uint, ICollection<uint>>(Owner, @event);
	}
}
