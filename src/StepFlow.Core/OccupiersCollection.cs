using System;
using System.Collections;
using System.Collections.Generic;
using StepFlow.Core.Exceptions;

namespace StepFlow.Core
{
	public class OccupiersCollection : IReadOnlyCollection<Piece>
	{
		public int Count => Items.Count;

		private HashSet<Piece> Items { get; } = new HashSet<Piece>();

		internal void Add(Piece piece)
		{
			if (piece is null)
			{
				throw new ArgumentNullException(nameof(piece));
			}

			if (!Items.Add(piece))
			{
				throw ExceptionBuilder.CreateAddExistsElement();
			}
		}

		internal void Remove(Piece piece)
		{
			if (piece is null)
			{
				throw new ArgumentNullException(nameof(piece));
			}

			if (!Items.Remove(piece))
			{
				throw ExceptionBuilder.CreateNotExistsElement();
			}
		}

		internal void Clear() => Items.Clear();

		public IEnumerator<Piece> GetEnumerator() => Items.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
