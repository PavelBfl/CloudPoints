using System.Numerics;

namespace StepFlow.Core.Tracks
{
	public class TrackUnit : Subject
	{
		public Vector2 Center { get; set; }

		public Vector2 Radius { get; set; }

		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));
	}
}
