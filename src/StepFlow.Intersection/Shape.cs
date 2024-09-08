using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	public sealed class Shape : IReadOnlyList<Rectangle>
	{
		private const int UNKNOWN_INDEX = -1;

		internal Shape(Segment owner, int index, ShapeRaw shapeRaw)
		{
			NullValidate.ThrowIfArgumentNull(owner, nameof(owner));
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			Owner = owner;
			Index = index;
			Rectangles = shapeRaw.Rectangles;
			Bounds = shapeRaw.Bounds;
		}

		public object? State { get; set; }

		public Rectangle this[int index] => Rectangles[index];

		public int Count => Rectangles.Length;

		private Segment Owner { get; }

		private int Index { get; set; }

		public Rectangle Bounds { get; }

		private Rectangle[] Rectangles { get; }

		public bool IntersectsWith(Shape other)
		{
			NullValidate.ThrowIfArgumentNull(other, nameof(other));

			if (Bounds.IntersectsWith(other.Bounds))
			{
				foreach (var rectangle in Rectangles)
				{
					foreach (var otherRectangle in other.Rectangles)
					{
						if (rectangle.IntersectsWith(otherRectangle))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		internal void Reset()
		{
			collisions = null;
		}

		private ReadOnlyCollection<Shape>? collisions;

		public ReadOnlyCollection<Shape> GetCollisions()
		{
			if (collisions is null)
			{
				var container = new List<Shape>(Owner.Count);

				foreach (var otherShape in Owner)
				{
					if (this != otherShape && IntersectsWith(otherShape))
					{
						container.Add(otherShape);
					}
				}

				collisions = Array.AsReadOnly(container.ToArray());
			}

			return collisions;
		}

		public bool IsEnable => Index != UNKNOWN_INDEX;

		public void Disable()
		{
			if (IsEnable)
			{
				Owner.RemoveAt(Index);
				Reset();
				Index = UNKNOWN_INDEX;
			}
		}

		public IEnumerator<Rectangle> GetEnumerator() => Rectangles.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
