using System;

namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommand : CommandVm
	{
		public CreateCommand(IContextElement context, GamePlay.Context gamePlayContext, NodeVm? begin)
			: base(context, new GamePlay.Commands.CreateCommand(gamePlayContext, new GamePlay.Strength(10), begin?.Source))
		{
			Begin = begin ?? throw new ArgumentNullException(nameof(begin));
		}

		public NodeVm Begin { get; }

		public override bool IsMark { get => Begin.IsMark; set => Begin.IsMark = value; }
	}
}
