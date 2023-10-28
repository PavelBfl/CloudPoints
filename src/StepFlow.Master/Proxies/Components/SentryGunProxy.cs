using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	[ComponentProxy(typeof(SentryGun), typeof(SentryGunProxy), "SentryGun")]
	public interface ISentryGunProxy
	{
		int Cooldown { get; }

		void CooldownDecrement();

		void CooldownReset();
	}

	internal class SentryGunProxy : ComponentProxy<SentryGun>, ISentryGunProxy
	{
		public SentryGunProxy(PlayMaster owner, SentryGun target) : base(owner, target)
		{
		}

		public int Cooldown { get => Target.Cooldown; private set => SetValue(x => x.Cooldown, value); }

		public void CooldownReset() => Cooldown = 100;

		public void CooldownDecrement()
		{
			if (Cooldown > 0)
			{
				Cooldown--;
			}
		}
	}
}
