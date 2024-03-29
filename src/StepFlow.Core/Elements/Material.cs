﻿using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;

namespace StepFlow.Core.Elements
{
	public class Material : ElementBase
	{
		public const string SHEDULER_CONTROL_NAME = "Control";

		private Scale? strength;

		public Scale? Strength { get => strength; set => SetComponent(ref strength, value); }

		private Collided? body;

		public Collided? Body { get => body; set => SetComponent(ref body, value); }

		public Collided GetBodyRequired() => PropertyRequired(Body, nameof(Body));

		public int Speed { get; set; }

		public ICollection<SchedulerRunner> Schedulers { get; } = new HashSet<SchedulerRunner>();

		public CourseVector? GetControlVector() => Schedulers.Select(x => x.Scheduler)
			.OfType<SchedulerVector>()
			.SelectMany(x => x.Vectors)
			.SingleOrDefault(x => x.Name == SHEDULER_CONTROL_NAME);

		public bool TryGetControlVector(out SchedulerRunner runner, out SchedulerVector scheduler, out CourseVector courseVector)
		{
			foreach (var localRunner in Schedulers)
			{
				if (localRunner.Scheduler is SchedulerVector schedulerVector)
				{
					foreach (var vector in schedulerVector.Vectors)
					{
						if (vector.Name == SHEDULER_CONTROL_NAME)
						{
							runner = localRunner;
							scheduler = schedulerVector;
							courseVector = vector;
							return true;
						}
					}
				}
			}

			runner = null;
			scheduler = null;
			courseVector = null;
			return false;
		}
	}
}
