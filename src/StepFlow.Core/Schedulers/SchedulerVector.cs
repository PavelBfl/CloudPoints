using System.Collections.Generic;
using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Schedulers
{
	public sealed class SchedulerVector : Scheduler
	{
		public static bool IsZero(Vector2 vector)
		{
			const float MAX_LENGTH_SQUARED = 0.0001f;

			return vector.LengthSquared() < MAX_LENGTH_SQUARED;
		}

		public Collided? Collided { get; set; }

		public Collided GetCollidedRequired() => PropertyRequired(Collided, nameof(Collided));

		public ICollection<CourseVector> Vectors { get; } = new List<CourseVector>();
	}
}
