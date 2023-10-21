using System.Collections.Generic;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISystemProxy
	{
		ICollection<IHandlerProxy> OnFrame { get; }
	}

	internal sealed class SystemProxy : ComponentProxy<Core.Components.System>, ISystemProxy
	{
		public SystemProxy(PlayMaster owner, Core.Components.System target) : base(owner, target)
		{
		}

		public ICollection<IHandlerProxy> OnFrame => CreateEvenProxy(Target.OnFrame);
	}
}
