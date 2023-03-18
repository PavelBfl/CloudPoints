using System;
using StepFlow.GamePlay;
using StepFlow.TimeLine;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandVm : WrapperVm<Command>, ICommand, IMarkered
	{
		public CommandVm(IServiceProvider serviceProvider, Command source)
			: base(serviceProvider, source)
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

		public virtual void Execute() => Source.Execute();

		public bool Prepare() => Source.Prepare();
	}
}
