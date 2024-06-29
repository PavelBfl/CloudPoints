﻿namespace StepFlow.Core.States
{
	public enum StateKind
	{
		Remove,
		Poison,
		Arc,
		Push,
		Dash,
	}

	public class State : Subject
	{
		public StateKind Kind { get; set; }

		public int? TotalCooldown { get; set; }

		public float Arg0 { get; set; }

		public float Arg1 { get; set; }

		public override bool Equals(object obj) => obj is State other && Kind == other.Kind;

		public override int GetHashCode() => Kind.GetHashCode();
	}
}
