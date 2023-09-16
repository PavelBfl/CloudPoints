using System;

namespace StepFlow.Core.Components
{
	public struct Turn
	{
		public Turn(long duration, IComponentChild? executor = null)
		{
			Duration = duration >= 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration));
			Executor = executor;
		}

		public long Duration { get; }

		public IComponentChild? Executor { get; }
	}
}
