using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Intersection
{
	public sealed class ShapeContainer : ShapeBase, ICollection<Rectangle>
	{
		public ShapeContainer(Context context) : base(context)
		{
		}

		public ShapeContainer(Context context, IEnumerable<Rectangle> rectangles) : base(context)
		{
			SubRectangles.UnionWith(rectangles);
		}

		public ShapeContainer(Context context, ShapeContainer original) : base(context, original)
		{
			SubRectangles.UnionWith(original);
		}

		private HashSet<Rectangle> SubRectangles { get; } = new HashSet<Rectangle>();

		public override int Count => SubRectangles.Count;

		public bool IsReadOnly => false;

		private Rectangle? bounds;

		public override Rectangle Bounds => bounds ??= CreateBorder();

		private Rectangle CreateBorder()
		{
			Rectangle? result = null;

			foreach (var rectangle in this)
			{
				if (result is { } instance)
				{
					result = Rectangle.Union(instance, rectangle);
				}
				else
				{
					result = rectangle;
				}
			}

			return result ?? Rectangle.Empty;
		}

		protected override void Reset()
		{
			bounds = null;

			base.Reset();
		}

		public override IEnumerator<Rectangle> GetEnumerator() => SubRectangles.GetEnumerator();

		public void Add(Rectangle item)
		{
			SubRectangles.Add(item);
			Reset();
		}

		// TODO Реализован для добавления элементов в инициализаторе, не уверен что он нужен
		public void Add(IEnumerable<Rectangle> items)
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

		public bool Contains(Rectangle item) => SubRectangles.Contains(item);

		public void CopyTo(Rectangle[] array, int arrayIndex) => SubRectangles.CopyTo(array, arrayIndex);

		public bool Remove(Rectangle item)
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
