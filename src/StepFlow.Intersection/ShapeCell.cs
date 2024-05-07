using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

		private RectangleF border;

		public RectangleF Border
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
		public override RectangleF Bounds => Border;

		public override int Count => 1;

		public override IEnumerator<RectangleF> GetEnumerator() => new[] { Border }.AsEnumerable().GetEnumerator();

		public override void Offset(Point value) => border.Offset(value);

		public override ShapeBase Clone() => new ShapeCell(Context, this);
	}
}
