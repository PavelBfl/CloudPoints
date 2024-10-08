using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	public sealed class Shape : IReadOnlyList<Rectangle>, IEquatable<Shape>
	{
		public static Shape Create(IEnumerable<Rectangle> rectangles)
		{
			NullValidate.ThrowIfArgumentNull(rectangles, nameof(rectangles));

			return new Shape(rectangles);
		}

		public static Shape Create(params Rectangle[] rectangles)
		{
			NullValidate.ThrowIfArgumentNull(rectangles, nameof(rectangles));

			return new Shape(rectangles);
		}

		private const int UNKNOWN_INDEX = -1;

		private static Rectangle CreateBounds(Rectangle[] rectangles)
		{
			Rectangle? result = null;
			foreach (var rectangle in rectangles)
			{
				result = result.HasValue ? Rectangle.Union(result.GetValueOrDefault(), rectangle) : rectangle;
			}

			return result ?? Rectangle.Empty;
		}

		private Shape(IEnumerable<Rectangle> rectangles)
		{
			NullValidate.ThrowIfArgumentNull(rectangles, nameof(rectangles));

			Rectangles = rectangles.ToArray();
			Bounds = CreateBounds(Rectangles);
		}

		public object? State { get; set; }

		public Rectangle this[int index] => Rectangles[index];

		public int Count => Rectangles.Length;

		public Context? Context => Owner?.Context;

		internal Segment? Owner { get; private set; }

		internal int Index { get; set; }

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

		private IReadOnlyList<Shape>? collisions;

		public IReadOnlyList<Shape> GetCollisions()
		{
			if (Owner is null)
			{
				return Array.Empty<Shape>();
			}

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

		internal void SetOwner(Segment owner, int index)
		{
			NullValidate.ThrowIfArgumentNull(owner, nameof(owner));
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			if (Owner is { })
			{
				throw ExceptionBuilder.CreateShapeAlreadyContext();
			}

			Owner = owner;
			Index = index;
		}

		public void Disable()
		{
			if (Owner is { })
			{
				Owner.RemoveAt(Index);
				Owner = null;
				Reset();
				Index = UNKNOWN_INDEX;
			}
		}

		public IEnumerator<Rectangle> GetEnumerator() => Rectangles.AsEnumerable().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public bool Equals(Shape? other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			else if (other is null)
			{
				return false;
			}
			else
			{
				if (Context != other.Context || Count != other.Count)
				{
					return false;
				}

				for (var i = 0; i < Count; i++)
				{
					if (this[i] != other[i])
					{
						return false;
					}
				}

				return true;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Shape other && Equals(other);
		}

		public override int GetHashCode()
		{
			var result = new HashCode();

			foreach (var rectangle in this)
			{
				result.Add(rectangle);
			}

			return result.ToHashCode();
		}

		public static bool operator ==(Shape? a, Shape? b)
		{
			if (a is { })
			{
				return a.Equals(b);
			}
			else if (b is { })
			{
				return b.Equals(a);
			}
			else
			{
				return true;
			}
		}

		public static bool operator !=(Shape? a, Shape? b) => !(a == b);
	}
}
