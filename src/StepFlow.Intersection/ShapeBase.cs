using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public abstract class ShapeBase : IReadOnlyList<Rectangle>, ICloneable
	{
		protected ShapeBase()
		{
		}

		protected ShapeBase(ShapeBase original)
		{
			if (original is null)
			{
				throw new ArgumentNullException(nameof(original));
			}

			Attached = original.Attached;
		}

		public object? Attached { get; set; }

		public abstract Rectangle this[int index] { get; }

		public abstract int Count { get; }

		internal int Index { get; set; } = -1;

		internal bool IsHandle { get; set; } = false;

		public abstract Rectangle Bounds { get; }

		public abstract void Offset(Point value);

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
	}
}
