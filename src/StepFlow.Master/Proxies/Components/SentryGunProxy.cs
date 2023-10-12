using StepFlow.Master.Proxies.Components.Custom;

namespace StepFlow.Master.Proxies.Components
{
	public interface ISentryGunProxy
	{
		IComponentProxy? Vision { get; set; }
	}

	internal class SentryGunProxy : ComponentProxy<SentryGun>, ISentryGunProxy
	{
		public SentryGunProxy(PlayMaster owner, SentryGun target) : base(owner, target)
		{
		}

		public IComponentProxy? Vision { get => (IComponentProxy?)Owner.CreateProxy(Target.Vision); set => SetValue(x => x.Vision, value?.Target); }
	}
}
