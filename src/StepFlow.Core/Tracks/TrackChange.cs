using System.Numerics;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public class TrackChange : Subject
	{
		public TrackChange()
		{
		}

		public TrackChange(TrackChangeDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			View = original.View;
			Thickness = original.Thickness;
			Size = original.Size;
			Position = original.Position;
		}

		public TrackView View { get; set; }

		public float Thickness { get; set; }

		public Vector2 Size { get; set; }

		public Vector2 Position { get; set; }
	}
}
