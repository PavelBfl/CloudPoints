using System;

namespace StepFlow.GamePlay.Commands
{
	public class CreateCommand : Command
	{
		public CreateCommand(Context owner, Strength original, float collisionDamage, Node? begin)
			: base(owner)
		{
			Strength = original ?? throw new ArgumentNullException(nameof(original));
			CollisionDamage = collisionDamage;
			Begin = begin;
		}

		public Strength Strength { get; }

		public float CollisionDamage { get; }

		public Node? Begin { get; }

		public Piece? Piece { get; private set; }

		public override void Execute()
		{
			Piece ??= new Piece(Strength.Max)
			{
				CollisionDamage = CollisionDamage,
			};
			Owner.World.Pieces.Add(Piece);
			Piece.Current = Begin;
		}

		public override void Revert()
		{
			if (Piece is { })
			{
				Owner.World.Pieces.Remove(Piece);
			}
		}
	}
}
