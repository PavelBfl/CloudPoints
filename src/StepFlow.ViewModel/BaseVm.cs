using System;
using StepFlow.Common;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public class BaseVm : BaseNotifyer
	{
		public BaseVm(LockProvider lockProvider)
		{
			LockProvider = lockProvider ?? throw new ArgumentNullException(nameof(lockProvider));
		}

		public LockProvider LockProvider { get; }
	}
}
