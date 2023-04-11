using System.Collections.Generic;
using System.Linq;
using StepFlow.Core.Collision;

namespace StepFlow.Core
{
	public class World<TNode, TPiece> : IWorld
		where TNode : Node
		where TPiece : Piece
	{
		public World()
		{
			Pieces = new PiecesCollection<TPiece>(this);
			Place = new Place<TNode>(this);
		}

		public PiecesCollection<TPiece> Pieces { get; }

		public Place<TNode> Place { get; }

		private static IEnumerable<PairCollision<TPiece>> GetSwaps(TPiece[] pieces)
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
						yield return new PairCollision<TPiece>(firstPiece, secondPiece);
					}
				}
			}
		}

		private static IEnumerable<CrashCollision<TPiece>> GetCrashes(TPiece[] pieces)
		{
			for (var iFirst = 0; iFirst < pieces.Length; iFirst++)
			{
				for (var iSecond = 0; iSecond < iFirst; iSecond++)
				{
					var firstPiece = pieces[iFirst];
					var secondPiece = pieces[iSecond];
					if (CheckCrash(firstPiece, secondPiece))
					{
						yield return new CrashCollision<TPiece>(firstPiece, secondPiece);
					}
					else if (CheckCrash(secondPiece, firstPiece))
					{
						yield return new CrashCollision<TPiece>(secondPiece, firstPiece);
					}
				}
			}
		}

		private static IEnumerable<IReadOnlyList<TPiece>> GetCompetitors(TPiece[] pieces)
		{
			var disputedNodes = new Dictionary<Node, List<TPiece>>();
			foreach (var piece in pieces)
			{
				if (piece.Next is { })
				{
					if (!disputedNodes.TryGetValue(piece.Next, out var competitors))
					{
						competitors = new List<TPiece>();
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

		public CollisionResult<TPiece> TakeStep()
		{
			var pieces = Pieces.ToArray();

			var result = new CollisionResult<TPiece>(
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
