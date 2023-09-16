namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class CollisionHandler : ComponentMaster, ICollisionHandler
	{
		public CollisionHandler(PlayMaster owner) : base(owner)
		{
		}

		public void Collision(ISubjectProxy main, ISubjectProxy other)
		{
			if (main.GetComponent(Master.Components.Names.STRENGTH) is IScaleProxy scale &&
				other.GetComponent(Master.Components.Names.DAMAGE) is ICollisionDamageProxy damage)
			{
				scale.Add(-damage.Damage);

				((ICollidedProxy?)main.GetComponent(Master.Components.Names.COLLIDED))?.Break();
			}
		}
	}
}
