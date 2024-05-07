using System;

namespace StepFlow.Core.Components
{
	public readonly struct Turn : IEquatable<Turn>
	{
		public Turn(int duration, Subject? executor = null)
		{
			Duration = duration >= 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration));
			Executor = executor;
		}

		public int Duration { get; }

		public Subject? Executor { get; }

		public Turn Decrement() => new Turn(Duration - 1, Executor);

		public bool Equals(Turn other) => Duration == other.Duration && Executor == other.Executor;

		public override bool Equals(object obj) => obj is Turn turn && Equals(turn);

		public override int GetHashCode() => HashCode.Combine(Duration, Executor);

		public static bool operator ==(Turn left, Turn right) => left.Equals(right);

		public static bool operator !=(Turn left, Turn right) => !(left == right);
	}
}
