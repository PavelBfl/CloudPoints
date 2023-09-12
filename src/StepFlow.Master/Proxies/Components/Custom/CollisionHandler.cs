using StepFlow.Core;

namespace StepFlow.Master.Proxies.Components.Custom
{
	internal sealed class CollisionHandler : IHandler
	{
		public void Handle(IComponentProxy component, string eventName, object? args)
		{
			if (component.GetComponent(Playground.STRENGTH_NAME) is IScaleProxy scale &&
				((ISubjectProxy?)args)?.GetComponent(Playground.COLLISION_DAMAGE_NAME) is ICollisionDamageProxy damage)
			{
				scale.Add(-damage.Damage);
			}

			((ICollidedProxy?)component.GetComponent(Playground.COLLIDED_NAME))?.Break();
		}
	}
}
