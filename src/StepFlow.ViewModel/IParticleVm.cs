using System;
using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public interface IParticleVm : IDisposable
	{
		ContextVm Owner { get; }

		void TakeStep();

		internal Particle Source { get; }
	}
}
