using StepFlow.Core;

namespace StepFlow.ViewModel
{
	public interface IParticleVm
	{
		WorldVm? Owner { get; }

		internal Particle? Source { get; }
	}
}
