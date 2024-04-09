using System;
using StepFlow.Intersection;

namespace StepFlow.Core.Components
{
	public sealed class CollidedAttached
	{
		public CollidedAttached(string propertyName, Collided collided)
		{
			PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
			Collided = collided ?? throw new ArgumentNullException(nameof(collided));
		}

		public string PropertyName { get; }

		public Collided Collided { get; }
	}

	public sealed class Collided : ComponentBase
	{
		public Collided()
		{
			Current.Attached = new CollidedAttached(nameof(Current), this);
			Next.Attached = new CollidedAttached(nameof(Next), this);
		}

		public ShapeContainer Current { get; } = new ShapeContainer(Playground.IntersectionContext);

		public ShapeContainer Next { get; } = new ShapeContainer(Playground.IntersectionContext);

		public bool IsMove { get; set; }

		public bool IsRigid { get; set; }
	}
}
