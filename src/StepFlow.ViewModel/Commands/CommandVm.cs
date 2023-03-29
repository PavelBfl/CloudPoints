using StepFlow.GamePlay;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandVm : WrapperVm<Command>, IMarkered
	{
		public CommandVm(IContextElement context, Command source)
			: base(context, source)
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
