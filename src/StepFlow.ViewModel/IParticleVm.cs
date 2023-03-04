using System;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public interface IParticleVm : IDisposable
	{
		WorldVm Owner { get; }

		void TakeStep();

		internal Particle Source { get; }
	}
}
