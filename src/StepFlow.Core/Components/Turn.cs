using System;

namespace StepFlow.Core.Components
{
	public readonly struct Turn
	{
		public Turn(long duration, Handler? executor = null)
		{
			Duration = duration >= 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration));
			Executor = executor;
		}

		public long Duration { get; }

		public Handler? Executor { get; }
	}
}
