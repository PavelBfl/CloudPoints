using System;

namespace StepFlow.Master.Proxies
{
	internal class ReadOnlyBaseProxy<TTarget> : IReadOnlyProxyBase<TTarget>
	{
		public ReadOnlyBaseProxy(PlayMaster owner, TTarget target)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}

		public PlayMaster Owner { get; }

		public TTarget Target { get; }
	}
}
