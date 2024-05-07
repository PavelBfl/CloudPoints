using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Intersection
{
	public sealed class ShapeContainer : ShapeBase, ICollection<RectangleF>
	{
		public ShapeContainer(Context context) : base(context)
		{
		}

		public ShapeContainer(Context context, IEnumerable<RectangleF> rectangles) : base(context)
		{
			SubRectangles.UnionWith(rectangles);
		}

		public ShapeContainer(Context context, ShapeContainer original) : base(context, original)
		{
			SubRectangles.UnionWith(original);
		}

		private HashSet<RectangleF> SubRectangles { get; } = new HashSet<RectangleF>();

		public override int Count => SubRectangles.Count;

		public bool IsReadOnly => false;

		private RectangleF? bounds;

		public override RectangleF Bounds => bounds ??= CreateBorder();

		private RectangleF CreateBorder()
		{
			RectangleF? result = null;

			foreach (var rectangle in this)
			{
				if (result is { } instance)
				{
					result = RectangleF.Union(instance, rectangle);
				}
				else
				{
					result = rectangle;
				}
			}

			return result ?? RectangleF.Empty;
		}

		protected override void Reset()
		{
			bounds = null;

			base.Reset();
		}

		public override IEnumerator<RectangleF> GetEnumerator() => SubRectangles.GetEnumerator();

		public void Add(RectangleF item)
		{
			SubRectangles.Add(item);
			Reset();
		}

		// TODO Реализован для добавления элементов в инициализаторе, не уверен что он нужен
		public void Add(IEnumerable<RectangleF> items)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void Clear()
		{
			SubRectangles.Clear();
			Reset();
		}

		public bool Contains(RectangleF item) => SubRectangles.Contains(item);

		public void CopyTo(RectangleF[] array, int arrayIndex) => SubRectangles.CopyTo(array, arrayIndex);

		public bool Remove(RectangleF item)
		{
			var removed = SubRectangles.Remove(item);
			if (removed)
			{
				Reset();
			}
			return removed;
		}

		public override void Offset(Point value)
		{
			if (value == Point.Empty)
			{
				return;
			}

			var subRectangles = SubRectangles.ToArray();
			SubRectangles.Clear();
			for (var i = 0; i < Count; i++)
			{
				var subRectangle = subRectangles[i];
				subRectangle.Offset(value);
				subRectangles[i] = subRectangle;
			}

			SubRectangles.UnionWith(subRectangles);
		}

		public override ShapeBase Clone() => new ShapeContainer(Context, this);
	}
}
