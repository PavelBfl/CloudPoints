using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
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
			}
		}
	}
}
