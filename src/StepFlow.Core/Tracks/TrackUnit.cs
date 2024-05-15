using System.Drawing;

namespace StepFlow.Core.Tracks
{
	public class TrackUnit : Subject
	{
		public RectangleF Bounds { get; set; }

		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));
	}
}
