using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Master.Proxies.Elements
{
	public interface IPlaygroundSwitchProxy : IMaterialProxy<PlaygroundSwitch>
	{
		string Destination { get; set; }
	}

	internal sealed class PlaygroundSwitchProxy : MaterialProxy<PlaygroundSwitch>, IPlaygroundSwitchProxy
	{
		public PlaygroundSwitchProxy(PlayMaster owner, PlaygroundSwitch target) : base(owner, target)
		{
		}

		public string Destination { get => Target.Destination; set => SetValue(value); }

		public override void Collision(CollidedAttached thisCollided, Material otherMaterial, CollidedAttached otherCollided)
		{
			if (otherMaterial is PlayerCharacter && otherCollided.PropertyName == nameof(Collided.Current))
			{
				Owner.NextPlayground = Destination;
			}
		}
	}
}
