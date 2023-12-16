using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public sealed class ShapeContainer : ShapeBase, IList<Rectangle>
	{
		public ShapeContainer()
		{
		}

		public ShapeContainer(IEnumerable<Rectangle> rectangles)
		{
			SubRectangles.AddRange(rectangles);
		}

		public ShapeContainer(ShapeContainer original) : base(original)
		{
			SubRectangles.AddRange(original);
		}

		private List<Rectangle> SubRectangles { get; } = new List<Rectangle>();

		public override int Count => SubRectangles.Count;

		public bool IsReadOnly => false;

		Rectangle IList<Rectangle>.this[int index]
		{
			get => SubRectangles[index];
			set
			{
				SubRectangles[index] = value;
				Reset();
			}
		}

		public override Rectangle this[int index] => SubRectangles[index];

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

		private void Reset()
		{
			bounds = null;
			IsHandle = false;
		}

		public override IEnumerator<Rectangle> GetEnumerator() => SubRectangles.GetEnumerator();

		public int IndexOf(Rectangle item) => SubRectangles.IndexOf(item);

		public void Insert(int index, Rectangle item)
		{
			SubRectangles.Insert(index, item);
			Reset();
		}

		public void RemoveAt(int index)
		{
			SubRectangles.RemoveAt(index);
			Reset();
		}

		public void Add(Rectangle item)
		{
			SubRectangles.Add(item);
			Reset();
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
			for (var i = 0; i < Count; i++)
			{
				var subRectangle = this[i];
				subRectangle.Offset(value);
				((IList<Rectangle>)this)[i] = subRectangle;
			}
		}

		public override ShapeBase Clone() => new ShapeContainer(this);
	}
}
