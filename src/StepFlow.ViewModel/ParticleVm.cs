using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.ViewModel.Collections;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public interface IParticleVm
	{
		
	}

	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : GamePlay.IParticle
	{
		public ParticleVm(ContextVm owner, T source)
			: base(owner, source)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			Commands = new CommandsCollectionVm(WrapperProvider, Source.Commands);
			CommandsCompleted = new CommandsCompletedCollection(this, Source.Commands.Where(x => !x.IsCompleted));
		}

		public ContextVm Owner { get; }

		public CommandsCollectionVm Commands { get; }

		public CommandsCompletedCollection CommandsCompleted { get; }

		public virtual void Refresh()
		{
		}

		public sealed class CommandsCompletedCollection : WrapperObserver<CommandVm, GamePlay.Command>
		{
			public CommandsCompletedCollection(IContextElement context, IEnumerable<GamePlay.Command> items) : base(items)
			{
				Context = context ?? throw new ArgumentNullException(nameof(context));
			}

			private IContextElement Context { get; }

			protected override CommandVm CreateObserver(GamePlay.Command observable)
				=> Context.WrapperProvider.GetOrCreateCommand(observable);
		}
	}
}
