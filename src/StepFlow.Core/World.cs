using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StepFlow.CollectionsNodes;

namespace StepFlow.Core
{
	public class World
	{
		public World(int colsCount, int rowsCount, HexOrientation orientation, bool offsetOdd)
		{
			if (colsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(colsCount));
			}

			if (rowsCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(rowsCount));
			}

			Orientation = orientation;
			OffsetOdd = offsetOdd;
			Particles = new ParticlesCollection(this);
			Place = new Place(this);

			for (var iCol = 0; iCol < colsCount; iCol++)
			{
				for (var iRow = 0; iRow < rowsCount; iRow++)
				{
					var position = new System.Drawing.Point(iCol, iRow);
					Place.Add(new Node(null, position));
				}
			}
		}

		public HexOrientation Orientation { get; }

		public bool OffsetOdd { get; }

		public ParticlesCollection Particles { get; }

		public Place Place { get; }

		public void TakeStep()
		{
			var pieces = Particles.OfType<Piece>().ToArray();

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
						SwapCollision(firstPiece, secondPiece);
					}
				}
			}

			for (var iFirst = 0; iFirst < pieces.Length; iFirst++)
			{
				for (var iSecond = 0; iSecond < iFirst; iSecond++)
				{
					var firstPiece = pieces[iFirst];
					var secondPiece = pieces[iSecond];
					if (CheckCrash(firstPiece, secondPiece))
					{
						CrashCollision(firstPiece, secondPiece);
					}
					else if (CheckCrash(secondPiece, firstPiece))
					{
						CrashCollision(secondPiece, firstPiece);
					}
				}
			}

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
				DisputedCollision(competitors);
			}

			foreach (var particle in Particles)
			{
				particle.TakeStep();
			}
		}

		private bool CheckCrash(Piece stationary, Piece moved)
			=> stationary.Current is { } && stationary.Next is null && moved.Next == stationary.Current;

		protected virtual void CrashCollision(Piece stationary, Piece moved)
		{
		}

		protected virtual void SwapCollision(Piece first, Piece second)
		{
		}

		protected virtual void DisputedCollision(IReadOnlyList<Piece> competitors)
		{
		}
	}
}
