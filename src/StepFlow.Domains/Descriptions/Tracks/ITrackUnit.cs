using System.Numerics;
using StepFlow.Common;

namespace StepFlow.Domains.Descriptions.Tracks
{
	public interface ITrackUnit : ISubject
	{
		public static void CopyTo(ITrackUnit source, ITrackUnit destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			ISubject.CopyTo(source, destination);

			destination.Center = source.Center;
			destination.Radius = source.Radius;
			PropertyCopyTo(source.GetClonerChange(), destination.GetClonerChange());
		}


		Vector2 Center { get; set; }

		Vector2 Radius { get; set; }

		IClonerProperty<ITrackChange> GetClonerChange();
	}
}
