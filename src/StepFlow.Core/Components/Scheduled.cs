using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace StepFlow.Core.Components
{
	public sealed class Scheduled : Component
	{
		public long QueueBegin { get; set; }

		public List<Turn> Queue { get; } = new List<Turn>();
	}

	public abstract class Turn
	{
		protected Turn(long duration)
			=> Duration = duration >= 0 ? duration : throw new ArgumentOutOfRangeException(nameof(duration));

		public long Duration { get; }

		public abstract void Execute();
	}
}
