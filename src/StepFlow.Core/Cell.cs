using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core
{
	public abstract class Cell : IBorderedChild
	{
		private Rectangle border;

		public Rectangle Border
		{
			get => border;
			set
			{
				if (Border != value)
				{
					border = value;
					ChangeBorder();
				}
			}
		}

		protected abstract void ChangeBorder();

		public IEnumerable<IBordered>? Childs => null;

		public void Offset(Point point)
		{
			border.Offset(point);
			ChangeBorder();
		}
	}
}
