using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.GamePlay.Commands;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : GamePlay.IParticle
	{
		public ParticleVm(T source)
			: base(source)
		{
			Commands = new CommandsCollectionVm(Source.Commands);
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= WrapperProvider.GetOrCreate<ContextVm>(Source.Owner.Owner);

		public CommandsCollectionVm Commands { get; }

		public virtual void Refresh()
		{
			Commands.Refresh();

			foreach (var command in Commands)
			{
				command.Refresh();
			}
		}

		public sealed class CommandsCompletedCollection : WrapperObserver<CommandVm, Command>
		{
			public CommandsCompletedCollection(IEnumerable<Command> items) : base(items)
			{
			}

			protected override CommandVm CreateObserver(Command observable)
				=> WrapperProvider.GetOrCreate<CommandVm>(observable);
		}
	}
}
