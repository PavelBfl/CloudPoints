using StepFlow.Core;
using StepFlow.Core.Tracks;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies.Tracks
{
	public interface ITrackBuilderProxy : IProxyBase<TrackBuilder>
	{
		TrackChange Change { get; }

		TrackChange? GetTrackForBuild()
		{
			var cooldownProxy = (IScaleProxy)Owner.CreateProxy(Target.GetCooldownRequired());

			if (cooldownProxy.Value == 0)
			{
				cooldownProxy.SetMax();
				return Target.GetChangeRequired();
			}
			else
			{
				cooldownProxy.Decrement();
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
	}
}
