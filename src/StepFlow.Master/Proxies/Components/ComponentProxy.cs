using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	internal class ComponentProxy<TComponent> : ProxyBase<TComponent>, IComponentProxy
		where TComponent : class, IComponentChild
	{
		public ComponentProxy(PlayMaster owner, TComponent target) : base(owner, target)
		{
		}

		public string? Name => Target.Site?.Name;

		public ISubjectProxy Subject => (ISubjectProxy)Owner.CreateProxy(Target.Site.Container);

		IComponentChild IProxyBase<IComponentChild>.Target => Target;

		protected ICollection<IHandlerProxy> CreateEvenProxy(ICollection<Handler> @event) => new EventProxy(Owner, @event);
	}
}
