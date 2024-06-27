using System.Numerics;
using StepFlow.Core.States;

namespace StepFlow.Master.Proxies.States
{
	public interface IStateProxy<TState> : IProxyBase<TState>
		where TState : State
	{
		int? TotalCooldown { get; set; }

		float Arg0 { get; set; }

		float Arg1 { get; set; }

		Vector2 Vector
		{
			get => new Vector2(Arg0, Arg1);
			set
			{
				Arg0 = value.X;
				Arg1 = value.Y;
			}
		}
	}

	internal class StateProxy<TState> : ProxyBase<TState>, IStateProxy<TState>
		where TState : State
	{
		public StateProxy(PlayMaster owner, TState target) : base(owner, target)
		{
		}

		public int? TotalCooldown { get => Target.TotalCooldown; set => SetValue(value); }

		public float Arg0 { get => Target.Arg0; set => SetValue(value); }

		public float Arg1 { get => Target.Arg1; set => SetValue(value); }
	}
}
