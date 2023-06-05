using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public class ParticleVm<TParticle> : WrapperVm<TParticle>, IParticleVm
		where TParticle : Particle
	{
		public ParticleVm(LockProvider wrapperProvider, TParticle source)
			: base(wrapperProvider, source)
		{
		}

		private PlaygroundVm? owner;

		public PlaygroundVm Owner => owner ??= LockProvider.GetOrCreate<PlaygroundVm>(Source.Owner);

		private bool isMark;

		public bool IsMark { get => isMark; set => SetValue(ref isMark, value); }

		private CommandsCollectionVm<Particle>? commands;

		public CommandsCollectionVm<Particle> Commands => commands ??= LockProvider.GetOrCreate<CommandsCollectionVm<Particle>>(Source.Commands);

		public override void SourceHasChange()
		{
			Commands.SourceHasChange();

			foreach (var command in Commands)
			{
				command.SourceHasChange();
			}
		}

		public override IEnumerable<ILockable> GetContent() => base.GetContent().ConcatIfNotNull(owner);
	}
}
