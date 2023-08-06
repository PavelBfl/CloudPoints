using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StepFlow.Core
{
	public class Bordered : IBorderedNode
	{
		public Bordered()
		{
		}

		public Bordered(Bordered original, Bordered? owner)
		{
			if (original is null)
			{
				throw new ArgumentNullException(nameof(original));
			}

			Owner = owner;

			foreach (var child in original.childs)
			{
				childs.Add(child.Clone(this));
			}
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

		private HashSet<IBorderedNode> childs = new HashSet<IBorderedNode>();

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

		public bool Remove(IBorderedNode border)
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

		public IBorderedNode Clone(Bordered? owner) => new Bordered(this, owner);

		private sealed class ChildCell : Cell
		{
			public ChildCell(Bordered? owner) => Owner = owner;

			private ChildCell(ChildCell original, Bordered? owner)
				: base(original)
			{
				Owner = owner;
			}

			public Bordered? Owner { get; }

			public override IBorderedNode Clone(Bordered? owner) => new ChildCell(this, owner);

			protected override void ChangeBorder() => Owner?.Refresh();
		}
	}
}
