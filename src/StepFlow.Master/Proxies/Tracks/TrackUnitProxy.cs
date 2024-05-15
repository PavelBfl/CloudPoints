using System.Drawing;
using StepFlow.Core.Tracks;

namespace StepFlow.Master.Proxies.Tracks
{
	public interface ITrackUnitProxy : IProxyBase<TrackUnit>
	{
		RectangleF Bounds { get; set; }

		bool Change()
		{
			var change = Target.GetChangeRequired();

			var size = new SizeF(
				Bounds.Width + change.Size.X,
				Bounds.Height + change.Size.Y
			);

			if (size.Width < 1 || size.Height < 1)
			{
				return false;
			}

			var location = new PointF(
				Bounds.X + change.Position.X,
				Bounds.Y + change.Position.Y
			);

			Bounds = new RectangleF(location, size);
			return true;
		}
	}

	internal class TrackUnitProxy : ProxyBase<TrackUnit>, ITrackUnitProxy
	{
		public TrackUnitProxy(PlayMaster owner, TrackUnit target) : base(owner, target)
		{
		}

		public RectangleF Bounds { get => Target.Bounds; set => SetValue(value); }
	}
}
