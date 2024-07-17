using System.Numerics;
using StepFlow.Domains;
using StepFlow.Domains.Tracks;

namespace StepFlow.Core.Tracks
{
	public sealed class TrackChange : Subject
	{
		public TrackChange(IContext context)
			: base(context)
		{
		}

		public TrackChange(IContext context, TrackChangeDto original)
			: base(context, original)
		{
			CopyExtensions.ThrowIfOriginalNull(original);

			View = original.View;
			Thickness = original.Thickness;
			Size = original.Size;
			Position = original.Position;
		}

		public TrackView View { get; set; }

		public float Thickness { get; set; }

		public Vector2 Size { get; set; }

		public Vector2 Position { get; set; }

		public override SubjectDto ToDto()
		{
			var result = new TrackChangeDto();
			CopyTo(result);
			return result;
		}

		public void CopyTo(TrackChangeDto container)
		{
			CopyExtensions.ThrowIfArgumentNull(container, nameof(container));

			base.CopyTo(container);

			container.View = View;
			container.Thickness = Thickness;
			container.Size = Size;
			container.Position = Position;
		}
	}
}
