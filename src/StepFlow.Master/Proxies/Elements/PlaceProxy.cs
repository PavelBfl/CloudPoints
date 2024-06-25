using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;
using StepFlow.Core.States;
using StepFlow.Master.Proxies.Components;
using StepFlow.Master.Proxies.States;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlaceProxy : IMaterialProxy<Place>
	{
		
	}

	internal class PlaceProxy : MaterialProxy<Place>, IPlaceProxy
	{
		public PlaceProxy(PlayMaster owner, Place target) : base(owner, target)
		{
		}

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial.Body == otherCollided.Collided && otherCollided.PropertyName == nameof(Collided.Current))
			{
				if (otherMaterial.States.SingleOrDefault(x => x.Kind == StateKind.Poison) is { } poisonState)
				{
					((IStateProxy<State>)Owner.CreateProxy(poisonState)).TotalCooldown++;
				}
				else
				{
					Owner.CreateCollectionProxy(otherMaterial.States).Add(new State()
					{
						Kind = StateKind.Poison,
						TotalCooldown = 1,
					});
				}



				var otherMaterialProxy = ((IMaterialProxy<Material>)Owner.CreateProxy(otherMaterial));

				var placeScheduler = otherMaterial.Schedulers
						.Select(x => x.Scheduler)
						.OfType<SchedulerLimit>()
						.SingleOrDefault(x => x.Name == Place.PLACE_SCHEDULER);

				if (placeScheduler is null)
				{
					placeScheduler = new SchedulerLimit()
					{
						Name = Place.PLACE_SCHEDULER,
						Range = new Scale()
						{
							Max = 1,
						},
						Source = new SchedulerDamage()
						{
							Material = otherMaterial,
							Damage = new Damage()
							{
								Value = 10,
							},
						},
					};

					otherMaterial.Schedulers.Add(new SchedulerRunner()
					{
						Scheduler = placeScheduler,
					});
				}

				if (placeScheduler.Current is null)
				{
					var rangeProxy = (IScaleProxy)Owner.CreateProxy(placeScheduler.GetRangeRequired());
					rangeProxy.Value = 0;
				} 
			}
		}
	}
}
