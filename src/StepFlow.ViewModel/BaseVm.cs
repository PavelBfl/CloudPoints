using System;
using StepFlow.Common;

namespace StepFlow.ViewModel
{
	public class BaseVm : BaseNotifyer
	{
		public BaseVm(WrapperProvider wrapperProvider)
		{
			WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
		}

		public WrapperProvider WrapperProvider { get; }
	}
}
