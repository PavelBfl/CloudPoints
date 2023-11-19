using System;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;

namespace StepFlow.Core
{
	public readonly struct CollisionPair
	{
		public CollisionPair(Material element, Collided component)
		{
			Element = element ?? throw new ArgumentNullException(nameof(element));
			Component = component ?? throw new ArgumentNullException(nameof(component));
		}

		public Material Element { get; }

		public Collided Component { get; }
	}
}
