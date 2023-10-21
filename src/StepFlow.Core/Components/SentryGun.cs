namespace StepFlow.Core.Components
{
	public class SentryGun : ComponentBase
	{
		public SentryGun(Playground owner) : base(owner)
		{
		}

		public int Cooldown { get; set; }
	}
}
