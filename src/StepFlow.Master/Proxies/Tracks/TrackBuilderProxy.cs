using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Tracks;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Tracks
{
	public interface ITrackBuilderProxy : IProxyBase<TrackBuilder>
	{
		TrackChange Change { get; }

		Scale Cooldown { get; set; }

		TrackChange? GetTrackForBuild()
		{
			if (Cooldown.Value == 0)
			{
				Cooldown = Cooldown.SetMax();
				return Target.GetChangeRequired();
			}
			else
			{
				Cooldown--;
				return null;
			}
		}
	}

	internal class TrackBuilderProxy : ProxyBase<TrackBuilder>, ITrackBuilderProxy
	{
		public TrackBuilderProxy(PlayMaster owner, TrackBuilder target) : base(owner, target)
		{
		}

		public TrackChange Change { get => Target.GetChangeRequired(); }

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }
	}
}
