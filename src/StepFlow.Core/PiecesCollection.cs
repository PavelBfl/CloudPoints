using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core
{
	public sealed class PiecesCollection<TPiece> : ICollection<TPiece>
		where TPiece : Piece
	{
		public PiecesCollection(IWorld owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		private IWorld Owner { get; }

		private HashSet<TPiece> Items { get; } = new HashSet<TPiece>();

		public void Add(TPiece item)
		{
			item.Owner = Owner;

			Items.Add(item);
		}

		public void Clear()
		{
			foreach (var item in Items)
			{
				item.Owner = null;
			}

			Items.Clear();
		}

		public bool Contains(TPiece item) => Items.Contains(item);

		public void CopyTo(TPiece[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

		public IEnumerator<TPiece> GetEnumerator() => Items.GetEnumerator();

		public bool Remove(TPiece item)
		{
			var removed = Items.Remove(item);
			if (removed)
			{
				item.Owner = null;
			}

			return removed;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
