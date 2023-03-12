using System;
using StepFlow.Core;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : Particle
	{
		public ParticleVm(IServiceProvider serviceProvider, ContextVm owner, T source)
			: base(serviceProvider, source)
		{
			this.owner = owner ?? throw new ArgumentNullException(nameof(owner));

			Owner.Particles.AddForce(this);
		}

		private ContextVm? owner;

		public ContextVm Owner => owner ?? throw new InvalidViewModelException();

		Particle IParticleVm.Source => Source;

		public virtual void Dispose()
		{
			Owner.Particles.RemoveForce(this);
			owner = null;
		}

		public virtual void TakeStep()
		{
			// Реализации нет т.к. для класса NodeVm пока нет реализации скорее всего в будущем метод будет абстрактным
		}
	}
}
