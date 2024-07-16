using StepFlow.Domains.Components;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public class TrackBuilder : Subject
	{
		public TrackBuilder()
		{
		}

		public TrackBuilder(TrackBuilderDto original)
			: base(original)
		{
			ThrowIfOriginalNull(original);

			Change = original.Change?.ToTrackChange();
			Cooldown = original.Cooldown;
		}

		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));

		public Scale Cooldown { get; set; }
	}
}
