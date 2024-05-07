using System.Numerics;
using StepFlow.Core.Components;

namespace StepFlow.Core.Actions
{
	public sealed class SetCourse : ActionBase
	{
		public Collided? Collided { get; set; }

		public Vector2 Course { get; set; }
	}
}
