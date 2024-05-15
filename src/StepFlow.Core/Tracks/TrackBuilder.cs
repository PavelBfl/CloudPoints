using StepFlow.Core.Components;

namespace StepFlow.Core.Tracks
{
	public class TrackBuilder : Subject
	{
		public TrackChange? Change { get; set; }

		public TrackChange GetChangeRequired() => PropertyRequired(Change, nameof(Change));

		public Scale? Cooldown { get; set; }

		public Scale GetCooldownRequired() => PropertyRequired(Cooldown, nameof(Cooldown));
	}
}
