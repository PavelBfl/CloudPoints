using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class Bordered : IBorderedChild
	{
		public Bordered()
		{
		}

		private Bordered(Bordered owner) => Owner = owner ?? throw new ArgumentNullException(nameof(owner));

		private Bordered? Owner { get; }

		private void Refresh()
		{
			border = null;
			Owner?.Refresh();
		}

		private Rectangle? border;

		public Rectangle Border
		{
			get
			{
				if (border is null)
				{
					foreach (var child in Childs)
					{
						border = border is null ? child.Border : Rectangle.Union(border.Value, child.Border);
					}

					border ??= Rectangle.Empty;
				}

				return border.Value;
			}
		}

		private HashSet<IBorderedChild> childs = new HashSet<IBorderedChild>();

		public IEnumerable<IBordered> Childs => childs;

		public void Offset(Point point)
		{
			foreach (var child in childs)
			{
				child.Offset(point);
			}
		}

		public Cell AddCell(Rectangle? border = null)
		{
			var result = new ChildCell(this)
			{
				Border = border ?? Rectangle.Empty,
			};
			childs.Add(result);
			return result;
		}

		public Bordered AddBorder()
		{
			var result = new Bordered(this);
			childs.Add(result);
			return result;
		}

		public bool Remove(IBorderedChild border)
		{
			if (border is null)
			{
				throw new ArgumentNullException(nameof(border));
			}

			var removed = childs.Remove(border);
			if (removed)
			{
				Refresh();
			}

			return removed;
		}

		private sealed class ChildCell : Cell
		{
			public ChildCell(Bordered owner) => Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			public Bordered Owner { get; }

			protected override void ChangeBorder() => Owner.Refresh();
		}
	}
}
