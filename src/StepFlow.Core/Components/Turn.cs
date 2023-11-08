using System;

namespace StepFlow.Core.Components
{
	public readonly struct Turn
	{
		public Turn(long duration, Subject? executor = null)
		{
			Duration = duration >= 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration));
			Executor = executor;
		}

		public long Duration { get; }

		public Subject? Executor { get; }
	}
}
