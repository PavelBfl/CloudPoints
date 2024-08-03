using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IWormholeProxy : IMaterialProxy<WormholeSwitch>
	{
		string Destination { get; set; }
	}

	internal sealed class WormholeProxy : MaterialProxy<WormholeSwitch>, IWormholeProxy
	{
		public WormholeProxy(PlayMaster owner, WormholeSwitch target) : base(owner, target)
		{
		}

		public string Destination { get => Target.Destination; set => SetValue(value); }

		public Vector2 Position { get => Target.Position; set => SetValue(value); }

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial is PlayerCharacter && otherCollided.PropertyName == nameof(Collided.Current))
			{
				Owner.NextPlayground = Destination;
				Owner.NextPosition = Position;
			}
		}
	}
}
