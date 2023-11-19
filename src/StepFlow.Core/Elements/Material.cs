using System.Collections.Generic;
using StepFlow.Core.Components;

namespace StepFlow.Core.Elements
{
	public class Material : Subject
	{
		public Scale? Strength { get; set; }

		public Collided? Body { get; set; }

		public IScheduled? Scheduler { get; set; }

		public virtual IEnumerable<Collided> GetCollideds()
		{
			if (Body is { })
			{
				yield return Body;
			}
		}
	}
}
