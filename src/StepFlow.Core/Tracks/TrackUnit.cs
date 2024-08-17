using System.Numerics;
using StepFlow.Common;
using StepFlow.Domains;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public sealed class TrackUnit : Subject
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

		public TrackChange GetChangeRequired() => NullValidate.PropertyRequired(Change, nameof(Change));

		public override SubjectDto ToDto()
		{
			var result = new TrackUnitDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(TrackUnitDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.Center = Center;
			container.Radius = Radius;
			container.Change = (TrackChangeDto?)Change?.ToDto();
		}
	}
}
