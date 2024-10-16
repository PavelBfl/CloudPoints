using System.Collections.Generic;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Domains.Elements;
using StepFlow.Domains.Tracks;

namespace StepFlow.Domains.Descriptions.Elements
{
	public interface IRoute : ISubject
	{
		public static void CopyTo(IRoute source, IRoute destination)
		{
			NullValidate.ThrowIfArgumentNull(source, nameof(source));
			NullValidate.ThrowIfArgumentNull(destination, nameof(destination));

			ISubject.CopyTo(source, destination);

			CollectionReset(source.Path, destination.Path);
			destination.Offset = source.Offset;
			destination.Speed = source.Speed;
			destination.Pivot = source.Pivot;
			destination.Complete = source.Complete;
		}

		IList<Curve> Path { get; }

		float Offset { get; set; }

		float Speed { get; set; }

		Vector2 Pivot { get; set; }

		RouteComplete Complete { get; set; }
	}
}
