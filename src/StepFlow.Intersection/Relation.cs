using System;

namespace StepFlow.Intersection
{
	public sealed class Relation
	{
		public Relation(Shape left, Shape right)
		{
			Left = left ?? throw new ArgumentNullException(nameof(left));
			Right = right ?? throw new ArgumentNullException(nameof(right));
		}

		public Shape Left { get; }

		public Shape Right { get; }

		internal void Reset() => isCollision = null;

		private bool? isCollision;

		public bool IsCollision => isCollision ??= Left.IsIntersection(Right);
	}
}
