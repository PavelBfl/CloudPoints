using System.Collections.Generic;
using System.Numerics;
using StepFlow.Domains.Components;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;

namespace StepFlow.Domains.Elements
{
	public class MaterialDto : ElementBaseDto
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		public Scale Strength { get; set; }

		public CollidedDto? Body { get; set; }

		public int Speed { get; set; }

		public int Weight { get; set; }

		public float Elasticity { get; set; }

		public Vector2 Course { get; set; }

		public bool IsFixed { get; set; }

		public CollisionBehavior CollisionBehavior { get; set; }

		public ICollection<StateDto> States { get; } = new HashSet<StateDto>();

		public TrackBuilderDto? Track { get; set; }
	}
}
