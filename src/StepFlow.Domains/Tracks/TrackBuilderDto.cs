using StepFlow.Domains.Components;

namespace StepFlow.Domains.Tracks
{
	public class TrackBuilderDto : SubjectDto
	{
		public TrackChangeDto? Change { get; set; }

		public Scale Cooldown { get; set; }
	}
}
