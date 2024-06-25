using StepFlow.Core.States;

namespace StepFlow.Master.Proxies.States
{
	public interface IStateProxy<TState> : IProxyBase<TState>
		where TState : State
	{
		int? TotalCooldown { get; set; }
	}

	internal class StateProxy<TState> : ProxyBase<TState>, IStateProxy<TState>
		where TState : State
	{
		public StateProxy(PlayMaster owner, TState target) : base(owner, target)
		{
		}

		public int? TotalCooldown { get => Target.TotalCooldown; set => SetValue(value); }
	}
}
