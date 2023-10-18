using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISentryGunProxy
	{
	
	}

	internal class SentryGunProxy : ComponentProxy<SentryGun>, ISentryGunProxy
	{
		public SentryGunProxy(PlayMaster owner, SentryGun target) : base(owner, target)
		{
		}
	}
}
