using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;

namespace StepFlow.Core.Elements
{
	public class Material : ElementBase
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		public Material()
		{
		}

		public Material(MaterialDto original)
			: base(original)
		{
			Strength = original.Strength;
			Body = original.Body?.ToCollided();
			Speed = original.Speed;
			Weight = original.Weight;
			Course = original.Course;
			CollisionBehavior = original.CollisionBehavior;
			States.AddUniqueRange(original.States.Select(CopyExtensions.ToState));
			Track = original.Track?.ToTrackBuilder();
		}

		public Scale Strength { get; set; }

		private Collided? body;

		public Collided? Body { get => body; set => SetComponent(ref body, value); }

		public Collided GetBodyRequired() => PropertyRequired(Body, nameof(Body));

		public int Speed { get; set; }

		public int Weight { get; set; }

		public Vector2 Course { get; set; }

		public CollisionBehavior CollisionBehavior { get; set; }

		public ICollection<State> States { get; } = new HashSet<State>();

		public TrackBuilder? Track { get; set; }
	}
}
