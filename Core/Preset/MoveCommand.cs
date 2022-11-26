using System;
using StepFlow.TimeLine;

namespace StepFlow.Core.Preset
{
	public class MoveCommand : CommandBase
	{
		public MoveCommand(Piece source, HexNode nextNode)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));
		}

		public Piece Source { get; }

		public HexNode NextNode { get; }

		public override void Execute()
		{
			Source.Current = NextNode;
		}
	}
}
