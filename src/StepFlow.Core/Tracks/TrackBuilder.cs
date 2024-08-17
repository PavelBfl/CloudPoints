using StepFlow.Common;
using StepFlow.Domains;
using StepFlow.Domains.Components;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public sealed class TrackBuilder : Subject
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

		public TrackChange GetChangeRequired() => NullValidate.PropertyRequired(Change, nameof(Change));

		public Scale Cooldown { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new SubjectDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(TrackBuilderDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Change = (TrackChangeDto?)Change?.ToDto();
			container.Cooldown = Cooldown;
		}
	}
}
