using System.Numerics;

namespace StepFlow.Core.Tracks
{
	public class TrackChange
	{
		public TrackView View { get; set; }

		public float Thickness { get; set; }

		public Vector2 Size { get; set; }

		public Vector2 Position { get; set; }
	}
}
