using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StepFlow.Common;

namespace StepFlow.Intersection
{
	public abstract class ShapeBase : IReadOnlyCollection<Rectangle>, ICloneable, IEnabled
	{
		public static ShapeBase Create(Context context, Rectangle rectangle) => new ShapeCell(context, rectangle);

		public static ShapeBase Create(Context context, IEnumerable<Rectangle> rectangles)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (rectangles is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var rectanglesInstance = rectangles.ToArray();
			return rectanglesInstance.Length switch
			{
				1 => new ShapeCell(context, rectanglesInstance[0]),
				_ => new ShapeContainer(context, rectanglesInstance),
			};
		}

		protected ShapeBase(Context context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		protected ShapeBase(Context context, ShapeBase original) : this(context)
		{
			if (original is null)
			{
				throw new ArgumentNullException(nameof(original));
			}
		}

		public object? Attached { get; set; }

		public abstract int Count { get; }

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

		public abstract Rectangle Bounds { get; }

		protected virtual void Reset()
		{
			if (IsEnable)
			{
				Context.Reset(this); 
			}
		}

		public abstract void Offset(Point value);

		public abstract bool Contains(Rectangle rectangle);

		public abstract IEnumerator<Rectangle> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public abstract ShapeBase Clone();

		public ShapeBase Clone(Point offset)
		{
			var result = Clone();
			result.Offset(offset);
			return result;
		}

		object ICloneable.Clone() => Clone();

		public override string ToString() => Bounds.ToString();
	}
}
