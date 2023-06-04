using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
    public interface IParticleVm : IMarkered, ILockable
	{
		void SourceHasChange();
	}
}
