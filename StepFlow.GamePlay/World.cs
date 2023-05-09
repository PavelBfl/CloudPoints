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
			foreach (var piece in Pieces)
			{
				var command = piece.Commands[0];
				piece.Commands.RemoveAt(0);
				Owner.AxisTime.Add(command);
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
