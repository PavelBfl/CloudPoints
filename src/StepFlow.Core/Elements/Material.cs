using System.Collections.Generic;
using StepFlow.Core.Border;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Material : Subject, ICollided, IScheduled
	{
		public IScale? Strength { get; set; }

		public IBordered? Current { get; set; }

		public IBordered? Next { get; set; }

		public bool IsMove { get; set; }

		public long QueueBegin { get; set; }

		public IList<Turn> Queue { get; } = new List<Turn>();
	}
}
