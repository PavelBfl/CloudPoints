using StepFlow.Domains.Components;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public class TrackBuilder : Subject
	{
		public TrackBuilder(IContext context)
			: base(context)
		{
		}

		public TrackBuilder(IContext context, TrackBuilderDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			Change = original.Change?.ToTrackChange(Context);
			Cooldown = original.Cooldown;
		}

		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));

		public Scale Cooldown { get; set; }
	}
}
