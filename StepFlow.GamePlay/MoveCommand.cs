using System;

namespace StepFlow.GamePlay
{
	public class MoveCommand : Command
	{
		public MoveCommand(Piece target, Node next)
			: base(target ?? throw new ArgumentNullException(nameof(target)))
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public new Piece Target { get; }

		public Node Next { get; }

		public override void Execute()
		{
			Target.Next = Next;
		}
	}
}
