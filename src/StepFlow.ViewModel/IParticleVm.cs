using System;

namespace StepFlow.ViewModel
{
	public interface IParticleVm : IDisposable
	{
		ContextVm Owner { get; }

		void TakeStep();

		internal GamePlay.IParticle Source { get; }
	}
}
