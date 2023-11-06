using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace StepFlow.Core.Border
{
	public sealed class Bordered : IBordered
	{
		public Bordered()
		{
			Childs = new ChildsCollection(this);
		}

		public Bordered(Bordered original)
			: this()
		{
			if (original is null)
			{
				throw new ArgumentNullException(nameof(original));
			}

			foreach (var child in original.Childs)
			{
				Childs.Add(child.Clone());
			}
		}

		private void Refresh()
		{
			border = null;
			BorderChange?.Invoke(this, EventArgs.Empty);
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

		public void Offset(Point value)
		{
			foreach (var child in Childs)
			{
				child.Offset(value);
			}
		}

		public event EventHandler? BorderChange;

		public ICollection<IBordered> Childs { get; }

		IEnumerable<IBordered> IBordered.Childs => Childs;

		public IBordered Clone() => new Bordered(this);

		private sealed class ChildsCollection : ICollection<IBordered>
		{
			public ChildsCollection(Bordered owner)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
				Items = new HashSet<IBordered>();
			}

			public int Count => Items.Count;

			public bool IsReadOnly => false;

			private Bordered Owner { get; }

			private HashSet<IBordered> Items { get; }

			private void ChildBorderChange(object sender, EventArgs e) => Owner.Refresh();

			public void Add(IBordered item)
			{
				if (Items.Add(item))
				{
					item.BorderChange += ChildBorderChange;
				}
			}

			public void Clear()
			{
				foreach (var item in this)
				{
					item.BorderChange -= ChildBorderChange;
				}

				Items.Clear();
			}

			public bool Contains(IBordered item) => Items.Contains(item);

			public void CopyTo(IBordered[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

			public IEnumerator<IBordered> GetEnumerator() => Items.GetEnumerator();

			public bool Remove(IBordered item)
			{
				var removed = Items.Remove(item);
				if (removed)
				{
					item.BorderChange -= ChildBorderChange;
				}

				return removed;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
