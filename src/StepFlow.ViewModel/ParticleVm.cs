using System;
using StepFlow.Core;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm, IDisposable
		where T : Particle
	{
		public ParticleVm(IServiceProvider serviceProvider, WorldVm owner, T source)
			: base(serviceProvider, source)
		{
			this.owner = owner ?? throw new ArgumentNullException(nameof(owner));

			Owner.Particles.AddForce(this);
		}

		private WorldVm? owner;

		public WorldVm Owner => owner ?? throw new InvalidViewModelException();

		Particle IParticleVm.Source => Source;

		public void Dispose()
		{
			Owner.Particles.RemoveForce(this);
			owner = null;
		}
	}
}
