using System;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public class ParticleVm<T> : WrapperVm<T>, IParticleVm
		where T : Particle
	{
		public ParticleVm(WorldVm owner, T source)
			: base(source, true)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));

			Owner.Particles.Add(Source, this);
		}

		public WorldVm Owner { get; }
	}
}
