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
		public ParticleVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider, source)
		{
			Commands = new CommandsCollectionVm(WrapperProvider, Source.Commands);
			CommandsCompleted = new CommandsCompletedCollection(WrapperProvider, Source.Commands.Where(x => !x.IsCompleted));
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ??= WrapperProvider.GetOrCreate<ContextVm>(Source.Owner.Owner);

		public CommandsCollectionVm Commands { get; }

		public CommandsCompletedCollection CommandsCompleted { get; }

		public virtual void Refresh()
		{
			Commands.Refresh();
			CommandsCompleted.Refresh();

			foreach (var command in Commands)
			{
				command.Refresh();
			}
		}

		public sealed class CommandsCompletedCollection : WrapperObserver<CommandVm, Command>
		{
			public CommandsCompletedCollection(WrapperProvider wrapperProvider, IEnumerable<Command> items) : base(items)
			{
				WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
			}

			private WrapperProvider WrapperProvider { get; }

			protected override CommandVm CreateObserver(Command observable)
				=> WrapperProvider.GetOrCreate<CommandVm>(observable);
		}
	}
}
