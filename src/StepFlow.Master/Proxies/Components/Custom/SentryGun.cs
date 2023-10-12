using StepFlow.Core;
using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components.Custom
{
	public sealed class SentryGun : ComponentBase
	{
		public SentryGun(Playground owner) : base(owner)
		{
		}

		public IComponentChild? Vision { get; set; }
	}

	public sealed class SentryHandler : HandlerBase
	{
		public SentryHandler(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
		{
			
		}
	}
}
