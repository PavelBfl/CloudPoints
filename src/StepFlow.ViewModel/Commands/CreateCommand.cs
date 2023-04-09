using System;

namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommand : CommandVm
	{
		public CreateCommand(IContextElement context, GamePlay.Context gamePlayContext, NodeVm? next)
			: base(context, new GamePlay.Commands.CreateCommand(gamePlayContext, new GamePlay.Strength(10), next?.Source))
		{
			Next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public NodeVm Next { get; }

		public override bool IsMark { get => Next.IsMark; set => Next.IsMark = value; }
	}
}
