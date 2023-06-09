using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StepFlow.Core.Collision;
using StepFlow.Core.Commands;
using StepFlow.Core.Commands.Accessors;
using StepFlow.Core.Commands.Preset;
using StepFlow.TimeLine;

namespace StepFlow.Core
{
	public class Playground
	{
		public Playground(Axis<ITargetingCommand<object>>? axisTime = null)
		{
			AxisTime = axisTime ?? new Axis<ITargetingCommand<object>>();
			Pieces = new PiecesCollection(this);
			Place = new Place(this);
			Scheduler = new Scheduler<Playground>(this);
		}

		public Axis<ITargetingCommand<object>> AxisTime { get; }

		public long Time { get; private set; } = 0;

		public PiecesCollection Pieces { get; }

		public Place Place { get; }

		public object? Buffer { get; set; }

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

		public void TakeStep()
		{
			PushToAxis(Scheduler.Queue, Time);

			foreach (var node in Place.Values)
			{
				PushToAxis(node.Scheduler.Queue, Time);
			}

			foreach (var piece in Pieces)
			{
				PushToAxis(piece.Scheduler.Queue, Time);
			}

			var collision = GetCollision();

			foreach (var collisionUnit in collision)
			{
				var fullDamage = collisionUnit.Sum(x => x.CollisionDamage);

				foreach (var piece in collisionUnit)
				{
					var addResult = piece.Strength.Add(-(fullDamage - piece.CollisionDamage));
					if (addResult == StrengthState.Min)
					{
						Pieces.Remove(piece);
					}
					else
					{
						Clear(piece);
					}
				}
			}

			foreach (var piece in Pieces)
			{
				TakeStep(piece);
			}

			Time++;
		}

		private void PushToAxis<T>(IQueue<T> queue, long time)
			where T : class
		{
			if (queue.Dequeue() is { } commands)
			{
				foreach (var command in commands)
				{
					AxisTime.Add(command);
				}
			}
		}

		private void TakeStep(Piece piece)
		{
			if (piece.IsScheduledStep)
			{
				SetProperty(piece, x => x.Current, piece.Next);
				Clear(piece);
			}
		}

		private void Clear(Piece piece)
		{
			SetProperty(piece, x => x.Next, null);
			SetProperty(piece, x => x.IsScheduledStep, false);
		}

		private void SetProperty<TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> propertyExpression, TValue newValue)
			where TTarget : class
		{
			var builder = AccessorsExtensions.CreatePropertyBuilder(propertyExpression, newValue, TrueResolver.Instance);
			AxisTime.Add(builder.Build(target));
		}
	}
}
