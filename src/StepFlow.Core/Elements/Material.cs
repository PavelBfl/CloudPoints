using System;
using System.Collections.Generic;
using System.Numerics;
using StepFlow.Core.Components;
using StepFlow.Core.Schedulers;
using StepFlow.Core.States;
using StepFlow.Core.Tracks;

namespace StepFlow.Core.Elements
{
	public class Material : ElementBase
	{
		public const string SHEDULER_CONTROL_NAME = "Control";
		public const string SHEDULER_INERTIA_NAME = "Inertia";

		public const int MAX_WEIGHT = 1000;

		public int Ordinal { get; set; }


		private Scale? strength;

		public Scale? Strength { get => strength; set => SetComponent(ref strength, value); }

		private Collided? body;

		public Collided? Body { get => body; set => SetComponent(ref body, value); }

		public Collided GetBodyRequired() => PropertyRequired(Body, nameof(Body));

		public int Speed { get; set; }

		public int Weight { get; set; }

		public Vector2 Course { get; set; }

		public ICollection<State> States { get; } = new HashSet<State>();

		public ICollection<SchedulerRunner> Schedulers { get; } = new HashSet<SchedulerRunner>();

		public TrackBuilder? Track { get; set; }

		public CourseVectorPath? GetCourseVector(string vectorName)
		{
			if (vectorName is null)
			{
				throw new ArgumentNullException(nameof(vectorName));
			}

			foreach (var localRunner in Schedulers)
			{
				if (localRunner.Scheduler is SchedulerVector schedulerVector)
				{
					foreach (var vector in schedulerVector.Vectors)
					{
						if (vector.Name == vectorName)
						{
							return new CourseVectorPath(localRunner, schedulerVector, vector);
						}
					}
				}
			}

			return null;
		}

		public sealed class CourseVectorPath
		{
			public CourseVectorPath(SchedulerRunner runner, SchedulerVector scheduler, CourseVector courseVector)
			{
				Runner = runner ?? throw new ArgumentNullException(nameof(runner));
				Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
				CourseVector = courseVector ?? throw new ArgumentNullException(nameof(courseVector));
			}

			public SchedulerRunner Runner { get; }

			public SchedulerVector Scheduler { get; }

			public CourseVector CourseVector { get; }
		}
	}
}
