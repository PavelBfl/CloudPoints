using System.Collections.Generic;
using StepFlow.Core.Border;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public sealed class PlayerCharacter : Subject, ICollided, IScheduled, IScale
	{
		public PlayerCharacter(Context context) : base(context)
		{
		}

		public Bordered? Current { get; set; }

		public Bordered? Next { get; set; }

		public long QueueBegin { get; set; }

		public IList<Turn> Queue { get; } = new List<Turn>();

		public float Value { get; set; }

		public float Max { get; set; }
	}
}
