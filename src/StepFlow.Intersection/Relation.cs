using System;

namespace StepFlow.Intersection
{
	public sealed class Relation
	{
		public Relation(ShapeBase left, ShapeBase right)
		{
			Left = left ?? throw new ArgumentNullException(nameof(left));
			Right = right ?? throw new ArgumentNullException(nameof(right));
		}

		public ShapeBase Left { get; }

		public ShapeBase Right { get; }

		internal void Reset() => isCollision = null;

		private bool? isCollision;

		public bool IsCollision => isCollision ??= GetCollision();

		private bool GetCollision()
		{
			var leftBorder = Left.Bounds;
			var rightBorder = Right.Bounds;
			if (leftBorder.IntersectsWith(rightBorder))
			{
				foreach (var leftChild in Left)
				{
					foreach (var rightChild in Right)
					{
						if (leftChild.IntersectsWith(rightChild))
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
