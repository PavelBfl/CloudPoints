﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Intersection
{
	public sealed class ShapeCell : ShapeBase
	{
		public ShapeCell()
		{
		}

		public ShapeCell(Rectangle border) => Border = border;

		public ShapeCell(ShapeCell original) : base(original)
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
					IsHandle = false;
				}
			}
		}
		public override Rectangle Bounds => Border;

		public override int Count => 1;

		public override IEnumerator<Rectangle> GetEnumerator() => ((IEnumerable<Rectangle>)new[] { Border }).GetEnumerator();

		public override void Offset(Point value) => border.Offset(value);

		public override ShapeBase Clone() => new ShapeCell(this);
	}
}
