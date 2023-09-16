namespace StepFlow.Master.Proxies.Components.Custom
{
	internal class SetDamage : HandlerBase
	{
		public SetDamage(PlayMaster owner) : base(owner)
		{
		}

		protected override void HandleInner(IComponentProxy component)
		{
			var damage = (IDamageProxy)Subject.GetComponentRequired(Master.Components.Names.DAMAGE);

			if (component.Subject.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale)
			{
				scale.Add(-damage.Value);
			}
		}
	}
}
