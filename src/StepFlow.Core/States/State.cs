using StepFlow.Core.Components;

namespace StepFlow.Core.States
{
	public enum StateKind
	{
		Remove,
		Poison,
		Arc,
		Push,
		Dash,
		CreatingProjectile,

		MoveAndStop,
		MoveReflection,
		MoveCW,
		MoveCCW,

		EnemySerpentineForwardState,
		EnemySerpentineForwardStateAttack,
		EnemySerpentineForwardToBackward,
		EnemySerpentineBackwardState,
		EnemySerpentineBackwardStateAttack,
		EnemySerpentineBackwardToForward,
	}

	public class State : Subject
	{
		public StateKind Kind { get; set; }

		public bool Enable { get; set; } = true;

		public Scale Cooldown { get; set; }

		public int? TotalCooldown { get; set; }

		public float Arg0 { get; set; }

		public float Arg1 { get; set; }

		public override bool Equals(object obj) => obj is State other && Kind == other.Kind;

		public override int GetHashCode() => Kind.GetHashCode();

		public override string ToString() => Kind.ToString();
	}
}
