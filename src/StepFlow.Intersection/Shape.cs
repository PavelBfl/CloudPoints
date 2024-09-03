using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common;
using StepFlow.Common.Exceptions;

namespace StepFlow.Intersection
{
	public sealed class Shape : ICollection<Rectangle>, ICloneable, IEnabled
	{
		public static Shape Create(Context context, Rectangle rectangle) => new Shape(context) { rectangle };

		public static Shape Create(Context context, IEnumerable<Rectangle> rectangles)
		{
			NullValidate.ThrowIfArgumentNull(context, nameof(context));
			NullValidate.ThrowIfArgumentNull(rectangles, nameof(rectangles));

			var result = new Shape(context);
			foreach (var rectangle in rectangles)
			{
				result.Add(rectangle);
			}

			return result;
		}

		private enum ShapeState : byte
		{
			Empty,
			Bounds,
			RectanglesWithoutBounds,
			RectanglesWithBounds,
		}

		private static Rectangle Offset(Rectangle rectangle, Point offset)
		{
			rectangle.Offset(offset);
			return rectangle;
		}

		private static Rectangle CreateBounds(IEnumerable<Rectangle> rectangles)
		{
			Rectangle? result = null;
			foreach (var subRectangle in rectangles)
			{
				if (result.HasValue)
				{
					result = Rectangle.Union(result.GetValueOrDefault(), subRectangle);
				}
				else
				{
					result = subRectangle;
				}
			}

			return result ?? Rectangle.Empty;
		}

		public Shape(Context context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public Shape(Context context, Shape original) : this(context)
		{
			NullValidate.ThrowIfArgumentNull(original, nameof(original));

			state = original.state;
			bounds = original.bounds;
			subRectangles = original.subRectangles?.ToList();
		}

		private ShapeState state = ShapeState.Empty;

		private Rectangle bounds;

		private List<Rectangle>? subRectangles;

		public int Count => state switch
		{
			ShapeState.Empty => 0,
			ShapeState.Bounds => 1,
			ShapeState.RectanglesWithoutBounds => subRectangles.Count,
			ShapeState.RectanglesWithBounds => subRectangles.Count,
			_ => throw EnumNotSupportedException.Create(state),
		};

		public object? Attached { get; set; }

		public Context Context { get; }

		internal int Index { get; set; } = -1;

		public bool IsEnable => Index >= 0;

		public void Enable()
		{
			if (!IsEnable)
			{
				Context.Add(this);
			}
		}

		public void Disable()
		{
			if (IsEnable)
			{
				Context.Remove(this);
			}
		}

		public Rectangle Bounds
		{
			get
			{
				switch (state)
				{
					case ShapeState.Empty: return Rectangle.Empty;
					case ShapeState.Bounds: return bounds;
					case ShapeState.RectanglesWithoutBounds:
						bounds = CreateBounds(subRectangles);
						state = ShapeState.RectanglesWithBounds;
						return bounds;
					case ShapeState.RectanglesWithBounds: return bounds;
					default: throw EnumNotSupportedException.Create(state);
				}
			}
		}

		public bool IsReadOnly => false;

		private void Reset()
		{
			if (IsEnable)
			{
				Context.Reset(this); 
			}
		}

		public void Offset(Point value)
		{
			if (value == Point.Empty)
			{
				return;
			}

			switch (state)
			{
				case ShapeState.Empty:
					break;
				case ShapeState.Bounds:
					bounds = Offset(bounds, value);
					break;
				case ShapeState.RectanglesWithoutBounds:
					for (var i = 0; i < subRectangles.Count; i++)
					{
						subRectangles[i] = Offset(subRectangles[i], value);
					}
					break;
				case ShapeState.RectanglesWithBounds:
					for (var i = 0; i < subRectangles.Count; i++)
					{
						subRectangles[i] = Offset(subRectangles[i], value);
					}
					bounds = Offset(bounds, value);
					break;
				default: throw EnumNotSupportedException.Create(state);
			}
		}

		public bool Contains(Rectangle rectangle)
		{
			return state switch
			{
				ShapeState.Empty => false,
				ShapeState.Bounds => bounds == rectangle,
				ShapeState.RectanglesWithoutBounds => subRectangles.Contains(rectangle),
				ShapeState.RectanglesWithBounds => subRectangles.Contains(rectangle),
				_ => throw EnumNotSupportedException.Create(state),
			};
		}

		public IEnumerator<Rectangle> GetEnumerator() => state switch
		{
			ShapeState.Empty => Enumerable.Empty<Rectangle>().GetEnumerator(),
			ShapeState.Bounds => new[] { bounds }.AsEnumerable().GetEnumerator(),
			ShapeState.RectanglesWithoutBounds => subRectangles.GetEnumerator(),
			ShapeState.RectanglesWithBounds => subRectangles.GetEnumerator(),
			_ => throw EnumNotSupportedException.Create(state),
		};

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public Shape Clone() => new Shape(Context, this);

		public Shape Clone(Point offset)
		{
			var result = Clone();
			result.Offset(offset);
			return result;
		}

		object ICloneable.Clone() => Clone();

		public bool IsIntersection(Shape other)
		{
			NullValidate.ThrowIfArgumentNull(other, nameof(other));

			switch (state)
			{
				case ShapeState.Empty: return false;
				case ShapeState.Bounds:
					switch (other.state)
					{
						case ShapeState.Empty: return false;
						case ShapeState.Bounds: return bounds.IntersectsWith(other.bounds);
						case ShapeState.RectanglesWithoutBounds:
						case ShapeState.RectanglesWithBounds: return IsIntersect(bounds, other);
						default: throw EnumNotSupportedException.Create(other.state);
					}
				case ShapeState.RectanglesWithoutBounds:
				case ShapeState.RectanglesWithBounds:
					switch (other.state)
					{
						case ShapeState.Empty: return false;
						case ShapeState.Bounds: return IsIntersect(other.bounds, this);
						case ShapeState.RectanglesWithoutBounds:
						case ShapeState.RectanglesWithBounds: return IsIntersect(this, other);
						default: throw EnumNotSupportedException.Create(other.state);
					}
				default: throw EnumNotSupportedException.Create(state);
			}
		}

		private static bool IsIntersect(Rectangle cell, Shape shape)
		{
			if (cell.IntersectsWith(shape.Bounds))
			{
				foreach (var subRectangle in shape.subRectangles)
				{
					if (subRectangle.IntersectsWith(cell))
					{
						return true;
					}
				}
			}

			return false;
		}

		private static bool IsIntersect(Shape left, Shape right)
		{
			if (left.Bounds.IntersectsWith(right.Bounds))
			{
				foreach (var leftRectangle in left.subRectangles)
				{
					foreach (var rightRectangle in right.subRectangles)
					{
						if (leftRectangle.IntersectsWith(rightRectangle))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		public override string ToString() => Bounds.ToString();

		public void Add(Rectangle item)
		{
			switch (state)
			{
				case ShapeState.Empty:
					bounds = item;
					state = ShapeState.Bounds;
					break;
				case ShapeState.Bounds:
					subRectangles = new List<Rectangle>()
					{
						bounds,
						item,
					};
					bounds = Rectangle.Empty;
					state = ShapeState.RectanglesWithoutBounds;
					break;
				case ShapeState.RectanglesWithoutBounds:
				case ShapeState.RectanglesWithBounds:
					subRectangles.Add(item);
					state = ShapeState.RectanglesWithoutBounds;
					break;
				default: throw EnumNotSupportedException.Create(state);
			}

			Reset();
		}

		public void Clear()
		{
			state = ShapeState.Empty;
			bounds = Rectangle.Empty;
			subRectangles = null;
			Reset();
		}

		public void CopyTo(Rectangle[] array, int arrayIndex)
		{
			switch (state)
			{
				case ShapeState.Empty:
					break;
				case ShapeState.Bounds:
					array[arrayIndex] = bounds;
					break;
				case ShapeState.RectanglesWithoutBounds:
				case ShapeState.RectanglesWithBounds:
					subRectangles.CopyTo(array, arrayIndex);
					break;
				default: throw EnumNotSupportedException.Create(state);
			}
		}

		public bool Remove(Rectangle item)
		{
			switch (state)
			{
				case ShapeState.Empty: return false;
				case ShapeState.Bounds:
					if (item == bounds)
					{
						state = ShapeState.Empty;
						bounds = Rectangle.Empty;
						return true;
					}
					else
					{
						return false;
					}
				case ShapeState.RectanglesWithoutBounds:
				case ShapeState.RectanglesWithBounds:
					var removed = subRectangles.Remove(item);
					if (subRectangles.Count == 1)
					{
						bounds = subRectangles[0];
						subRectangles = null;
						state = ShapeState.Bounds;
					}

					return removed;
				default: throw EnumNotSupportedException.Create(state);
			}

			Reset();
		}
	}
}
