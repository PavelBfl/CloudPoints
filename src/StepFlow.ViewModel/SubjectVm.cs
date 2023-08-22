using StepFlow.Core;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public sealed class SubjectVm : WrapperVm<Subject>
	{
		public SubjectVm(LockProvider lockProvider, Subject source) : base(lockProvider, source)
		{
		}

		public bool IsSelect { get; set; }
	}
}
