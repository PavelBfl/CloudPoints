using System;

namespace StepFlow.ViewModel
{
	public class ParticleVm : WrapperVm<GamePlay.IParticle>
	{
		public ParticleVm(ContextVm owner, GamePlay.IParticle source)
			: base(owner, source)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			CommandQueue = new CommandsQueueVm(this);
		}

		public ContextVm Owner { get; }

		public CommandsQueueVm CommandQueue { get; }

		public virtual void Refresh()
		{
		}
	}
}
