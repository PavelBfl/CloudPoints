namespace StepFlow.ViewModel.Commands
{
	public sealed class CreateCommand : CommandVm
	{
		public CreateCommand(WrapperProvider wrapperProvider, GamePlay.Commands.CreateCommand source)
			: base(wrapperProvider, source)
		{
			Source = source;
		}

		private new GamePlay.Commands.CreateCommand Source { get; }

		private NodeVm? begin;

		public NodeVm Begin => begin ??= (NodeVm)WrapperProvider.GetViewModel(Source);

		public override bool IsMark { get => Begin.IsMark; set => Begin.IsMark = value; }
	}
}
