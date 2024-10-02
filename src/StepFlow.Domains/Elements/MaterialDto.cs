using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using StepFlow.Domains.States;
using StepFlow.Domains.Tracks;

namespace StepFlow.Domains.Elements
{
	public class MaterialDto : ElementBaseDto
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		#region Body
		public IList<Rectangle> BodyCurrent { get; } = new List<Rectangle>();

		public IList<Rectangle> BodyNext { get; } = new List<Rectangle>();

		public Vector2 BodyOffset { get; set; }

		public bool BodyIsMove { get; set; } 
		#endregion

		public bool IsRigid { get; set; }

		public Scale Strength { get; set; }

		public int Speed { get; set; }

		public int Weight { get; set; }

		public float Elasticity { get; set; }

		public Vector2 Course { get; set; }

		public bool IsFixed { get; set; }

		public ICollection<StateDto> States { get; } = new HashSet<StateDto>();

		public RouteDto? Route { get; set; }

		public TrackBuilderDto? Track { get; set; }
	}
}
