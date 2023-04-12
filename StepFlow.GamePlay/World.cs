using System;
using System.Linq;

namespace StepFlow.GamePlay
{
	public sealed class World : Core.World<Node, Piece>
	{
		public World(Context owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public Context Owner { get; }

		public void TakeStep()
		{
			var collision = base.GetCollision();

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
						piece.Clear();
					}
				}
			}

			foreach (var piece in Pieces)
			{
				piece.TakeStep();
			}
		}
	}
}
