using System.Collections.Generic;
using StepFlow.Core;
using StepFlow.Core.Commands;

namespace StepFlow.ViewModel
{
	public class ParticleVm<TCommand, TParticle, TParticleVm> : WrapperVm<TParticle>, IParticleVm
		where TCommand : ITargetingCommand<TParticle>
		where TParticle : Particle
		where TParticleVm : WrapperVm<TParticle>
	{
		public ParticleVm(WrapperProvider wrapperProvider, TParticle source)
			: base(wrapperProvider, source)
		{
			Commands = new CommandsCollectionVm<ITargetingCommand<Particle>, Particle, TParticleVm>(WrapperProvider, Source.Commands);
		}

		private PlaygroundVm? owner;

		public PlaygroundVm Owner => owner ??= WrapperProvider.GetOrCreate<PlaygroundVm>(Source.Owner);

		public CommandsCollectionVm<ITargetingCommand<Particle>, Particle, TParticleVm> Commands { get; }

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

		public override IEnumerable<IWrapper> GetContent() => base.GetContent().ConcatIfNotNull(owner);
	}
}
