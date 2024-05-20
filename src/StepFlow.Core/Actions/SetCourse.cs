using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Actions
{
	public sealed class SetCourse : ActionBase
	{
		public Collided? Collided { get; set; }

		public Collided GetCollidedRequired() => PropertyRequired(Collided, nameof(Collided));

		public Vector2 Course { get; set; }
	}
}
