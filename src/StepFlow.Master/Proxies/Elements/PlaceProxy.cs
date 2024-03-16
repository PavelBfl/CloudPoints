using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Schedulers;

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

		public override void Collision(Collided thisCollided, Material otherMaterial, Collided otherCollided)
		{
			var placeSceduler = (SchedulerLimit?)otherMaterial.Schedulers
				.Select(x => x.Scheduler)
				.SingleOrDefault(x => x.Name == Place.PLACE_SCHEDULER);

			if (placeSceduler is null)
			{
				placeSceduler = new SchedulerLimit()
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
					Scheduler = placeSceduler,
				});
			}

			if (placeSceduler.Current is null)
			{
				placeSceduler.Range.Value = 0;
			}
		}
	}
}
