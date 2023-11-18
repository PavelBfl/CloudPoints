using System.Collections;
using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Material : Subject
	{
		public IScale? Strength { get; set; }

		public ICollided? Body { get; set; }

		public IScheduled? Scheduler { get; set; }

		public virtual IEnumerable<ICollided> GetCollideds()
		{
			if (Body is { })
			{
				yield return Body;
			}
		}
	}
}
