using System;

namespace StepFlow.Core.Commands
{
	public class CreateCommand : Command<Playground>
	{
		public CreateCommand(Playground playground, Strength original, float collisionDamage, Node? begin)
			: base(playground)
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
			Piece ??= new Piece(Target)
			{
				CollisionDamage = CollisionDamage,
			};
			Target.Pieces.Add(Piece);
			Piece.Current = Begin;
		}

		public override void Revert()
		{
			if (Piece is { })
			{
				Target.Pieces.Remove(Piece);
			}
		}
	}
}
