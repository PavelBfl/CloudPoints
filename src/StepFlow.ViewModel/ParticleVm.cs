using System;

namespace StepFlow.ViewModel
{
	public class ParticleVm : WrapperVm<GamePlay.IParticle>
	{
		public ParticleVm(ContextVm owner, GamePlay.IParticle source)
			: base(owner, source)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

		public ContextVm Owner { get; }

		public virtual void TakeStep()
		{
			// Реализации нет т.к. для класса NodeVm пока нет реализации скорее всего в будущем метод будет абстрактным
		}
	}
}
