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

		private AccessorVm<ParticleVm> target;

		public ParticleVm? Target => target.GetValue(WrapperProvider, Source.Target);

		public abstract bool IsMark { get; set; }

		public override void Dispose()
		{
			base.Dispose();
			Source.Dispose();
		}
	}
}
