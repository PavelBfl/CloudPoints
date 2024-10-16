using System.Numerics;
using StepFlow.Common;
using StepFlow.Domains.Tracks;

namespace StepFlow.Domains.Descriptions.Tracks
{
	public interface ITrackChange : ISubject, IClonerTo<ITrackChange>
	{
		public static void CopyTo(ITrackChange source, ITrackChange destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			ISubject.CopyTo(source, destination);

			destination.View = source.View;
			destination.Thickness = source.Thickness;
			destination.Size = source.Size;
			destination.Position = source.Position;
		}

		TrackView View { get; set; }

		float Thickness { get; set; }

		Vector2 Size { get; set; }

		Vector2 Position { get; set; }

		void IClonerTo<ITrackChange>.CloneTo(ITrackChange container) => CopyTo(this, container);
	}
}
