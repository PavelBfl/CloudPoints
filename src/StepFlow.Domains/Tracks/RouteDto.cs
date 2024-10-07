using System.Collections.Generic;

namespace StepFlow.Domains.Tracks
{
	public class RouteDto : SubjectDto
	{
		public IList<Curve> Path { get; } = new List<Curve>();

		public float Offset { get; set; }

		public float Speed { get; set; }
	}
}
