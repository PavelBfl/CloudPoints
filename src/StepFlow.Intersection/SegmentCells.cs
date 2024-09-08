using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Intersection
{
	internal sealed class SegmentCells : Segment
	{
		public SegmentCells(Context context, Segment? owner, Rectangle bounds)
			: base(context, owner, bounds)
		{
			var halfLess = bounds.Size / 2;
			var halfGreater = bounds.Size - halfLess;
			var center = bounds.Location + halfLess;
			LeftTop = new Segment(context, this, new Rectangle(bounds.Location, halfLess));
			RightTop = new Segment(context, this, new Rectangle(center.X, bounds.Y, halfLess.Width, halfGreater.Height));
			LeftBottom = new Segment(context, this, new Rectangle(bounds.X, center.Y, halfLess.Width, halfGreater.Height));
			RightBottom = new Segment(context, this, new Rectangle(center, halfGreater));
		}

		private Segment LeftTop { get; }

		private Segment RightTop { get; }

		private Segment LeftBottom { get; }

		private Segment RightBottom { get; }

		public override void RemoveAt(int index)
		{
			base.RemoveAt(index);

			LeftTop.CommonReset();
			RightTop.CommonReset();
			LeftBottom.CommonReset();
			RightBottom.CommonReset();
		}

		public override Shape Add(ShapeRaw shapeRaw)
		{
			if (LeftTop.TryAdd(shapeRaw, out var shape) ||
				RightTop.TryAdd(shapeRaw, out shape) ||
				LeftBottom.TryAdd(shapeRaw, out shape) ||
				RightBottom.TryAdd(shapeRaw, out shape))
			{
				CommonReset();
				return shape;
			}
			else
			{
				LeftTop.CommonReset();
				RightTop.CommonReset();
				LeftBottom.CommonReset();
				RightBottom.CommonReset();
				return base.Add(shapeRaw);
			}
		}

		public override int Count => Common.Count + LeftTop.Count + RightTop.Count + LeftBottom.Count + RightBottom.Count;

		public override IEnumerator<Shape> GetEnumerator()
			=> Common.Concat(LeftTop).Concat(RightTop).Concat(LeftBottom).Concat(RightBottom).GetEnumerator();
	}
}
