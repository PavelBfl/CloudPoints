using StepFlow.Core.States;
using StepFlow.Domains.Components;
using StepFlow.Domains.States;

namespace StepFlow.Master.Scripts
{
	public struct StateParameters
	{
		public StateKind Kind { get; set; }

		public bool Enable { get; set; }

		public Scale Cooldown { get; set; }

		public int? TotalCooldown { get; set; }

		public float Arg0 { get; set; }

		public float Arg1 { get; set; }

		public readonly State ToState() => new State()
		{
			Kind = Kind,
			Enable = Enable,
			Cooldown = Cooldown,
			TotalCooldown = TotalCooldown,
			Arg0 = Arg0,
			Arg1 = Arg1,
		};
	}
}
