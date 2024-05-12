using StepFlow.Core.Components;
using System.Collections.Generic;
using System.Drawing;
using StepFlow.Core.Elements;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Elements
{
	public interface ITrackProxy : IProxyBase<Track>
	{
		IList<RectangleF> Steps { get; }

		float ScaleOffset { get; set; }

		Scale Cooldown { get; }

		void Offset(Rectangle current)
		{
			var stepsProxy = Owner.CreateListProxy(Steps);

			var i = 0;
			while (i < stepsProxy.Count)
			{
				var rect = stepsProxy[i];

				var size = rect.Size;
				size.Width -= ScaleOffset * 2;
				size.Height -= ScaleOffset * 2;

				if (size.Width > 0 && size.Height > 0)
				{
					var newRect = new RectangleF(
						new PointF(rect.X + ScaleOffset, rect.Y + ScaleOffset),
						size
					);
					stepsProxy[i] = newRect;
					i++;
				}
				else
				{
					stepsProxy.RemoveAt(i);
				}
			}

			var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Cooldown);
			if (Cooldown.Value == 0)
			{
				stepsProxy.Add(new RectangleF(current.Location, current.Size));

				cooldownProxy.SetMax();
			}
			else
			{
				cooldownProxy.Decrement();
			}
		}
	}

	internal class TrackProxy : ProxyBase<Track>, ITrackProxy
	{
		public TrackProxy(PlayMaster owner, Track target) : base(owner, target)
		{
		}

		public IList<RectangleF> Steps => Target.Steps;

		public float ScaleOffset { get => Target.ScaleOffset; set => SetValue(value); }

		public Scale Cooldown { get => Target.GetCooldownRequired(); }
	}
}
