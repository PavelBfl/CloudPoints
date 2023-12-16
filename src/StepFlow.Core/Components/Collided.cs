﻿using StepFlow.Intersection;

namespace StepFlow.Core.Components
{
	public sealed class Collided : ComponentBase
	{
		public ShapeBase? Current { get; set; }

		public ShapeBase? Next { get; set; }

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
