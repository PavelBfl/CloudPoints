using System;

namespace StepFlow.GamePlay.Commands
{
	public class MoveCommand : Command
	{
		public MoveCommand(Piece target, Node next)
			: base(target ?? throw new ArgumentNullException(nameof(target)))
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Next = next ?? throw new ArgumentNullException(nameof(next));
			Prev = (Node?)Target.Current;
		}

		public new Piece Target { get; }

		public Node Next { get; }

		public Node? Prev { get; }

		public override void Execute() => Target.Next = Next;

		public override void Revert() => Target.Current = Prev;
	}
}
