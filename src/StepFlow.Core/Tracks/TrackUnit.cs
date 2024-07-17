using System.Numerics;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public class TrackUnit : Subject
	{
		public TrackUnit(IContext context)
			: base(context)
		{
		}

		public TrackUnit(IContext context, TrackUnitDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Center = original.Center;
			Radius = original.Radius;
			Change = original.Change?.ToTrackChange(Context);
		}

		public Vector2 Center { get; set; }

		public Vector2 Radius { get; set; }

		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));
	}
}
