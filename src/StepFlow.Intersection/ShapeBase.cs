using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public abstract class ShapeBase : IReadOnlyCollection<RectangleF>, ICloneable
	{
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

			Attached = original.Attached;
		}

		public object? Attached { get; set; }

		public abstract int Count { get; }

		public Context Context { get; }

		internal int Index { get; set; } = -1;

		public abstract RectangleF Bounds { get; }

		protected virtual void Reset() => Context.Reset(this);

		public abstract void Offset(Point value);

		public abstract IEnumerator<RectangleF> GetEnumerator();

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
