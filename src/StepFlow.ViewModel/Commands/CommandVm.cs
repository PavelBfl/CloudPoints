using StepFlow.GamePlay.Commands;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandVm : WrapperVm<Command>, IMarkered
	{
		public CommandVm(WrapperProvider wrapperProvider, Command source)
			: base(wrapperProvider, source)
		{
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= WrapperProvider.GetOrCreate<ContextVm>(Source.Owner);

		private IParticleVm? target;

		public IParticleVm? Target => target ??= WrapperProvider.Get<IParticleVm?>(Source.Target);

		public abstract bool IsMark { get; set; }
	}
}
