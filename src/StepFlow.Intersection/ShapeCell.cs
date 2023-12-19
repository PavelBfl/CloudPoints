using System;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public sealed class ShapeCell : ShapeBase
	{
		public ShapeCell(Context context) : base(context)
		{
		}

		public ShapeCell(Context context, Rectangle border) : base(context)
		{
			Border = border;
		}

		public ShapeCell(Context context, ShapeCell original) : base(context, original)
		{
			Border = original.Border;
		}

		public override Rectangle this[int index] => index == 0 ? Border : throw new ArgumentOutOfRangeException(nameof(index));

		private Rectangle border;

		public Rectangle Border
		{
			get => border;
			set
			{
				if (Border != value)
				{
					border = value;
					Reset();
				}
			}
		}
		public override Rectangle Bounds => Border;

		public override int Count => 1;

		public override IEnumerator<Rectangle> GetEnumerator() => ((IEnumerable<Rectangle>)new[] { Border }).GetEnumerator();

		public override void Offset(Point value) => border.Offset(value);

		public override ShapeBase Clone() => new ShapeCell(Context, this);
	}
}
