using StepFlow.Core.Components;

namespace StepFlow.Master.Proxies.Components
{
	public interface ITurnProxy : IReadOnlyProxyBase<Turn>
	{
		long Duration { get; }

		ITurnExecutor? Executor { get; }
	}

	internal sealed class TurnProxy : ReadOnlyBaseProxy<Turn>, ITurnProxy
	{
		public TurnProxy(PlayMaster owner, Turn target) : base(owner, target)
		{
		}

		public long Duration => Target.Duration;

		public ITurnExecutor? Executor => (ITurnExecutor?)Owner.CreateProxy(Target.Executor);
	}
}
