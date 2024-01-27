using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Scheduler : ElementBase
	{
		public Subject? Target { get; set; }

		public int Begin { get; set; }

		public int CurrentIndex { get; set; }

		public IList<Turn> Queue { get; } = new List<Turn>();
	}
}
