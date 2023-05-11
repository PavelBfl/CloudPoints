using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class Node : DrawableGameComponent
	{
		public Node(Game game) : base(game)
		{
			Childs = new ChildsCollection(this);
		}

		private Node? owner;

		public Node? Owner
		{
			get => owner;
			internal set
			{
				if (Owner != value)
				{
					owner = value;

					if (Owner is null)
					{
						Game.Components.Remove(this);
					}
					else
					{
						Game.Components.Add(this);
					}
				}
			}
		}

		public ChildsCollection Childs { get; }

		public void Free()
		{
			Game.Components.Remove(this);

			foreach (var child in Childs)
			{
				child.Free();
			}
		}
	}

	public class ChildsCollection : Collection<Node>
	{
		public ChildsCollection(Node owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		private Node Owner { get; }

		protected override void ClearItems()
		{
			foreach (var child in this)
			{
				child.Owner = null;
			}

			base.ClearItems();
		}

		protected override void InsertItem(int index, Node item)
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

		protected override void SetItem(int index, Node item)
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
