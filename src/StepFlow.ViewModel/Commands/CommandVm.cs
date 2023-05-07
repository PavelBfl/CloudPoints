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

		public ContextVm Owner => owner ??= (ContextVm)WrapperProvider.GetViewModel(Source.Owner);

		private AccessorVm<IParticleVm> target;

		public IParticleVm? Target => target.GetValue(WrapperProvider, Source.Target);

		public bool IsComplete => Source.IsCompleted;

		public abstract bool IsMark { get; set; }

		public virtual void Refresh()
		{
		}
	}
}
