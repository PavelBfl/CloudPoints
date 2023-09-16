using System.Linq;

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
				other.GetComponent(Master.Components.Names.DAMAGE) is IDamageProxy damage)
			{
				if (!damage.Kind.Any())
				{
					scale.Add(-damage.Value);
				}
				else
				{
					if (damage.Kind.Contains(PlayMaster.FIRE_DAMAGE))
					{
						scale.Add(-damage.Value * 2);
					}

					if (damage.Kind.Contains(PlayMaster.POISON_DAMAGET))
					{
						var poisonSubject = Subject.Playground.CreateSubject();
						var damageSubject = (IDamageProxy)poisonSubject.AddComponent(Master.Components.Types.DAMAGE, Master.Components.Names.DAMAGE);
						damageSubject.Value = damage.Value / 2;
						damageSubject.Kind.Add(PlayMaster.POISON_DAMAGET);
						var setDamageHandler = (IHandler)poisonSubject.AddComponent(Master.Components.Handlers.SET_DAMAGE);
						var removeSubjectHandler = (IHandler)poisonSubject.AddComponent(Master.Components.Handlers.REMOVE_SUBJECT);
						var removeComponentHandler = (IHandler)poisonSubject.AddComponent(Master.Components.Handlers.REMOVE_COMPONENT);

						var poisonScheduler = (IScheduledProxy)Subject.AddComponent(Master.Components.Types.SCHEDULER);
						for (var i = 0; i < 5; i++)
						{
							poisonScheduler.Add(5, setDamageHandler);
						}
						poisonScheduler.Add(0, removeComponentHandler);
						poisonScheduler.Add(0, removeSubjectHandler);
					}
				}
			}

			((ICollidedProxy?)main.GetComponent(Master.Components.Names.COLLIDED))?.Break();
		}
	}
}
