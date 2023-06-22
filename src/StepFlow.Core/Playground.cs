using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Collision;
using StepFlow.Core.Commands;

namespace StepFlow.Core
{
	public class Playground : IScheduled<Playground>
	{
		public Playground()
		{
			Pieces = new PiecesCollection(this);
			Place = new Place(this);
			Scheduler = new Scheduler<Playground>(this);
		}

		public PiecesCollection Pieces { get; }

		public Place Place { get; }

		public IScheduler<Playground> Scheduler { get; }

		private static IEnumerable<PairCollision> GetSwaps(Piece[] pieces)
		{
			for (var iFirst = 0; iFirst < pieces.Length; iFirst++)
			{
				for (var iSecond = 0; iSecond < iFirst; iSecond++)
				{
					var firstPiece = pieces[iFirst];
					var secondPiece = pieces[iSecond];
					if (firstPiece.Current is { } && firstPiece.Next is { } &&
						secondPiece.Current is { } && secondPiece.Next is { } &&
						firstPiece.Current == secondPiece.Next && secondPiece.Current == firstPiece.Next
					)
					{
						yield return new PairCollision(firstPiece, secondPiece);
					}
				}
			}
		}

		private static IEnumerable<CrashCollision> GetCrashes(Piece[] pieces)
		{
			for (var iFirst = 0; iFirst < pieces.Length; iFirst++)
			{
				for (var iSecond = 0; iSecond < iFirst; iSecond++)
				{
					var firstPiece = pieces[iFirst];
					var secondPiece = pieces[iSecond];
					if (CheckCrash(firstPiece, secondPiece))
					{
						yield return new CrashCollision(firstPiece, secondPiece);
					}
					else if (CheckCrash(secondPiece, firstPiece))
					{
						yield return new CrashCollision(secondPiece, firstPiece);
					}
				}
			}
		}

		private static IEnumerable<IReadOnlyList<Piece>> GetCompetitors(Piece[] pieces)
		{
			var disputedNodes = new Dictionary<Node, List<Piece>>();
			foreach (var piece in pieces)
			{
				if (piece.Next is { })
				{
					if (!disputedNodes.TryGetValue(piece.Next, out var competitors))
					{
						competitors = new List<Piece>();
						disputedNodes[piece.Next] = competitors;
					}

					competitors.Add(piece);
				}
			}

			foreach (var competitors in disputedNodes.Values.Where(x => x.Count > 1))
			{
				yield return competitors;
			}
		}

		public CollisionResult GetCollision()
		{
			var pieces = Pieces.ToArray();

			var result = new CollisionResult(
				GetCrashes(pieces),
				GetSwaps(pieces),
				GetCompetitors(pieces)
			);

			return result;
		}

		private static bool CheckCrash(Piece stationary, Piece moved)
			=> stationary.Current is { } && stationary.Next is null && moved.Next == stationary.Current;
	}
}
