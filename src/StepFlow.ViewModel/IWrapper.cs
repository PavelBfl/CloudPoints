using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public interface IWrapper<out T> : ILockable
		where T : class
	{
		T Source { get; }

		void SourceHasChange();
	}
}
