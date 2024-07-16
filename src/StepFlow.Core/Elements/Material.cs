using System.Collections.Generic;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;

namespace StepFlow.Core.Elements
{
	public enum CollisionBehavior
	{
		None,
		Stop,
		Reflection,
		CW,
		CCW,
	}

	public class Material : ElementBase
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		public int Ordinal { get; set; }

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
