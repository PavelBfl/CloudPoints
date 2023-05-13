using System;
using System.Collections.ObjectModel;

namespace StepFlow.View.Sketch
{
	public class ChildsCollection : Collection<Primitive>
	{
		public ChildsCollection(Primitive owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private Primitive Owner { get; }

		protected override void ClearItems()
		{
			foreach (var child in this)
			{
				child.Owner = null;
			}

			base.ClearItems();
		}

		protected override void InsertItem(int index, Primitive item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (item.Owner is not null)
			{
				throw new InvalidOperationException();
			}

			item.Owner = Owner;

			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			this[index].Owner = null;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Primitive item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (item.Owner is not null)
			{
				throw new InvalidOperationException();
			}

			this[index].Owner = null;
			item.Owner = Owner;

			base.SetItem(index, item);
		}
	}
}
