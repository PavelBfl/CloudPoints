using System;

namespace StepFlow.Core.Commands.Preset
{
	public class MoveCommand : Command<Piece>
	{
		public MoveCommand(Piece target, Node? next)
			: base(target)
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));
			Prev = Target.Current;
		}

		public Node? Next { get; }

		public Node? Prev { get; }

		public override void Execute() => Target.Next = Next;

		public override void Revert() => Target.Current = Prev;
	}

	internal sealed class SetCurrentCommandBuilder : IBuilder<Piece>
	{
		public ITargetingCommand<Piece> Build(Piece target)
		{
			throw new NotImplementedException();
		}

		public bool CanBuild(Piece target)
		{
			throw new NotImplementedException();
		}
	}

	internal sealed class SetCurrentCommand : Command<Piece>
	{
		public SetCurrentCommand(Piece target) : base(target)
		{
		}

		public Node? OldValue { get; private set; }

		public Node? NewValue { get; private set; }

		public override void Execute()
		{
			OldValue = Target.Current;
			Target.Current = NewValue;
		}

		public override void Revert()
		{
			Target.Current = OldValue;
		}
	}
}
