using System;
using System.Collections;
using System.Collections.Generic;

namespace StepFlow.Core
{
	public sealed class PiecesCollection : Subject, ICollection<Piece>
	{
		public PiecesCollection(Playground owner) : base(owner)
		{
		}

		public int Count => Items.Count;

		public bool IsReadOnly => false;

		private HashSet<Piece> Items { get; } = new HashSet<Piece>();

		public void Add(Piece item)
		{
			CheckInteractionRequired(item);

			Items.Add(item);
		}

		public void Clear() => Items.Clear();

		public bool Contains(Piece item) => Items.Contains(item);

		public void CopyTo(Piece[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

		public IEnumerator<Piece> GetEnumerator() => Items.GetEnumerator();

		public bool Remove(Piece item) => Items.Remove(item);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
