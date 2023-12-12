using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public abstract class ShapeBase : IReadOnlyList<Rectangle>
	{
		public abstract Rectangle this[int index] { get; }

		public abstract int Count { get; }

		internal int Index { get; set; } = -1;

		internal bool IsHandle { get; set; } = false;

		public abstract Rectangle Bounds { get; }

		public abstract IEnumerator<Rectangle> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
