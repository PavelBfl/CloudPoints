namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommand : CommandVm
	{
		public CreateCommand(GamePlay.Commands.CreateCommand source)
			: base(source)
		{
			Source = source;
		}

		private new GamePlay.Commands.CreateCommand Source { get; }

		private NodeVm? begin;

		public NodeVm? Begin => begin ??= Source.Begin?.GetOrCreate<NodeVm>();

		public override bool IsMark { get => Begin.IsMark; set => Begin.IsMark = value; }
	}
}
