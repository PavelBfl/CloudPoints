using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class CollisionHandler : ComponentMaster, ICollisionHandler
	{
		public CollisionHandler(PlayMaster owner) : base(owner)
		{
		}

		public void Collision(ISubjectProxy main, ISubjectProxy other)
		{
			if (main.GetComponent(Playground.STRENGTH_NAME) is IScaleProxy scale &&
				other.GetComponent(Playground.COLLISION_DAMAGE_NAME) is ICollisionDamageProxy damage)
			{
				scale.Add(-damage.Damage);
			}

			((ICollidedProxy?)main.GetComponent(Playground.COLLIDED_NAME))?.Break();
		}
	}
}
