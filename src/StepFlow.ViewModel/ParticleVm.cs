namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : GamePlay.IParticle
	{
		public ParticleVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider, source)
		{
			Commands = new CommandsCollectionVm(WrapperProvider, Source.Commands);
		}

		private WorldVm? owner;

		public WorldVm Owner => owner ??= WrapperProvider.GetOrCreate<WorldVm>(Source.Owner);

		public CommandsCollectionVm Commands { get; }

		private bool isMark;

		public bool IsMark { get => isMark; set => SetValue(ref isMark, value); }

		public override void Refresh()
		{
			Commands.Refresh();

			foreach (var command in Commands)
			{
				command.Refresh();
			}
		}
	}
}
