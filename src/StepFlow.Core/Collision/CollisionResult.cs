using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.Core.Collision
{
	public class CollisionResult<TPiece> : IReadOnlyCollection<IReadOnlyList<TPiece>>
		where TPiece : Piece
	{
		public CollisionResult(IEnumerable<CrashCollision<TPiece>> crashes, IEnumerable<PairCollision<TPiece>> swaps, IEnumerable<IReadOnlyList<TPiece>> competitors)
		{
			Crashes = crashes?.ToArray() ?? throw new ArgumentNullException(nameof(crashes));
			Swaps = swaps?.ToArray() ?? throw new ArgumentNullException(nameof(swaps));
			Competitors = competitors?.ToArray() ?? throw new ArgumentNullException(nameof(competitors));
		}

		public IReadOnlyList<CrashCollision<TPiece>> Crashes { get; }

		public IReadOnlyList<PairCollision<TPiece>> Swaps { get; }

		public IReadOnlyList<IReadOnlyList<TPiece>> Competitors { get; }

		public int Count => Crashes.Count + Swaps.Count + Competitors.Count;

		private IPiecesCollision<TPiece>? pieces;

		public IPiecesCollision<TPiece> Pieces => pieces ??= new PiecesCollision(this.SelectMany(x => x));

		public IEnumerator<IReadOnlyList<TPiece>> GetEnumerator() => Swaps.Union(Crashes).Union(Competitors).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class PiecesCollision : IPiecesCollision<TPiece>
		{
			public PiecesCollision(IEnumerable<TPiece> pieces) => Pieces = new HashSet<TPiece>(pieces);

			public int Count => Pieces.Count;

			private HashSet<TPiece> Pieces { get; }

			public bool Contains(TPiece piece) => Pieces.Contains(piece);

			public IEnumerator<TPiece> GetEnumerator() => Pieces.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
