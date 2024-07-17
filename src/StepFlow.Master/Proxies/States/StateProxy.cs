﻿using System.Numerics;
using StepFlow.Core.States;
using StepFlow.Domains.Components;

namespace StepFlow.Master.Proxies.States
{
	public interface IStateProxy : IProxyBase<State>
	{
		bool Enable { get; set; }

		Scale Cooldown { get; set; }

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

	internal class StateProxy : ProxyBase<State>, IStateProxy
	{
		public StateProxy(PlayMaster owner, State target) : base(owner, target)
		{
		}

		public bool Enable { get => Target.Enable; set => SetValue(value); }

		public Scale Cooldown { get => Target.Cooldown; set => SetValue(value); }

		public int? TotalCooldown { get => Target.TotalCooldown; set => SetValue(value); }

		public float Arg0 { get => Target.Arg0; set => SetValue(value); }

		public float Arg1 { get => Target.Arg1; set => SetValue(value); }
	}
}
