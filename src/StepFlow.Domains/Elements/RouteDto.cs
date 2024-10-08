using System.Collections.Generic;
using System.Numerics;
using StepFlow.Domains.Tracks;

namespace StepFlow.Domains.Elements
{
	public enum RouteComplete
	{
		None,
		Remove,
	}

	public class RouteDto : SubjectDto
	{
		public IList<Curve> Path { get; } = new List<Curve>();

		public float Offset { get; set; }

		public float Speed { get; set; }

		public Vector2 Pivot { get; set; }

		public RouteComplete Complete { get; set; }
	}
}
