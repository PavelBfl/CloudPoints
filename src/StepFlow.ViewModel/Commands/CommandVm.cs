using System.Collections.Generic;
using StepFlow.GamePlay.Commands;

namespace StepFlow.ViewModel.Commands
{
	public abstract class CommandVm : WrapperVm<Command>, IMarkered
	{
		public CommandVm(Command source)
			: base(source)
		{
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= Source.Owner.GetOrCreate<ContextVm>();

		private IParticleVm? target;

		public IParticleVm? Target => Source.Target?.GetOrCreate<IParticleVm>();

		public abstract bool IsMark { get; set; }
	}
}
