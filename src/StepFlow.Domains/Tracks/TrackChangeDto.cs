using System.Numerics;

namespace StepFlow.Domains.Tracks
{
	public class TrackChangeDto : SubjectDto
	{
		public TrackView View { get; set; }

		public float Thickness { get; set; }

		public Vector2 Size { get; set; }

		public Vector2 Position { get; set; }
	}
}
