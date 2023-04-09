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

		protected override void ExecuteInner()
		{
			var piece = new Piece(Strength.Max);
			Owner.World.Pieces.Add(piece);
			piece.Next = Begin;
		}
	}
}
