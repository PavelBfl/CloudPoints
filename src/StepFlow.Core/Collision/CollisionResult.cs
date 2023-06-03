using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.Core.Collision
{
	public class CollisionResult : IReadOnlyCollection<IReadOnlyList<Piece>>
	{
		public CollisionResult(IEnumerable<CrashCollision> crashes, IEnumerable<PairCollision> swaps, IEnumerable<IReadOnlyList<Piece>> competitors)
		{
			Crashes = crashes?.ToArray() ?? throw new ArgumentNullException(nameof(crashes));
			Swaps = swaps?.ToArray() ?? throw new ArgumentNullException(nameof(swaps));
			Competitors = competitors?.ToArray() ?? throw new ArgumentNullException(nameof(competitors));
		}

		public IReadOnlyList<CrashCollision> Crashes { get; }

		public IReadOnlyList<PairCollision> Swaps { get; }

		public IReadOnlyList<IReadOnlyList<Piece>> Competitors { get; }

		public int Count => Crashes.Count + Swaps.Count + Competitors.Count;

		private IPiecesCollision<Piece>? pieces;

		public IPiecesCollision<Piece> Pieces => pieces ??= new PiecesCollision(this.SelectMany(x => x));

		public IEnumerator<IReadOnlyList<Piece>> GetEnumerator() => Swaps.Union(Crashes).Union(Competitors).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private sealed class PiecesCollision : IPiecesCollision<Piece>
		{
			public PiecesCollision(IEnumerable<Piece> pieces) => Pieces = new HashSet<Piece>(pieces);

			public int Count => Pieces.Count;

			private HashSet<Piece> Pieces { get; }

			public bool Contains(Piece piece) => Pieces.Contains(piece);

			public IEnumerator<Piece> GetEnumerator() => Pieces.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}
}
