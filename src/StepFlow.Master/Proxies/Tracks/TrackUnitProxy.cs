using System.Drawing;
using System.Numerics;
using StepFlow.Core.Tracks;

namespace StepFlow.Master.Proxies.Tracks
{
	public interface ITrackUnitProxy : IProxyBase<TrackUnit>
	{
		Vector2 Center { get; set; }

		Vector2 Radius { get; set; }

		bool Change()
		{
			var change = Target.GetChangeRequired();

			Radius = Radius + change.Size;
			Center = Center + change.Position;

			return Radius.X > 1 && Radius.Y > 1;
		}
	}

	internal class TrackUnitProxy : ProxyBase<TrackUnit>, ITrackUnitProxy
	{
		public TrackUnitProxy(PlayMaster owner, TrackUnit target) : base(owner, target)
		{
		}

		public Vector2 Center { get => Target.Center; set => SetValue(value); }

		public Vector2 Radius { get => Target.Radius; set => SetValue(value); }
	}
}
