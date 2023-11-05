using System;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core.Border
{
	public sealed class Cell : IBordered
	{
		public Cell()
		{
		}

		public Cell(Cell original) => Border = original.Border;

		private Rectangle border;

		public Rectangle Border
		{
			get => border;
			set
			{
				if (Border != value)
				{
					border = value;
					BorderChange?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public void Offset(Point value)
		{
			var newBorder = border;
			newBorder.Offset(value);
			Border = newBorder;
		}

		public event EventHandler? BorderChange;

		public IEnumerable<IBordered>? Childs => null;

		public IBordered Clone() => new Cell(this);
	}
}
