using System;
using StepFlow.Core;

namespace StepFlow.GamePlay
{
	public class MoveCommand : Command
	{
		public MoveCommand(Context owner, Piece target, Node next)
			: base(owner, target ?? throw new ArgumentNullException(nameof(target)))
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
