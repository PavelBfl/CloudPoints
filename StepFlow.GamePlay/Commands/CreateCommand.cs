using System;

namespace StepFlow.GamePlay.Commands
{
	public class CreateCommand : Command
	{
		public CreateCommand(Context owner, Strength original, Node? begin)
			: base(owner)
		{
			Strength = original ?? throw new ArgumentNullException(nameof(original));
			Begin = begin;
		}

		public Strength Strength { get; }

		public Node? Begin { get; }

		public Piece? Piece { get; private set; }

		public override void Execute()
		{
			Piece ??= new Piece(Strength.Max);
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
