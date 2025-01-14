﻿using System.Linq;
using StepFlow.Core.Elements;
using StepFlow.Core.States;
using StepFlow.Domains.States;
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

		protected override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial == otherCollided.Material && otherCollided.Name == nameof(Material.Collided.Current))
			{
				if (otherMaterial.States.SingleOrDefault(x => x.Kind == StateKind.Poison) is { } poisonState)
				{
					((IStateProxy)Owner.CreateProxy(poisonState)).TotalCooldown++;
				}
				else
				{
					Owner.CreateCollectionProxy(otherMaterial.States).Add(new State(Owner.Playground.Context)
					{
						Kind = StateKind.Poison,
						TotalCooldown = 1,
					});
				}
			}
		}
	}
}
