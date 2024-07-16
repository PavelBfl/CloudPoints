using System.Numerics;

namespace StepFlow.Domains.Tracks
{
	public class TrackUnitDto : SubjectDto
	{
		public Vector2 Center { get; set; }

		public Vector2 Radius { get; set; }

		public TrackChangeDto? Change { get; set; }
	}
}
