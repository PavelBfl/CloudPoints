using System;
using StepFlow.Common;

namespace StepFlow.ViewModel
{
	public class BaseVm : BaseNotifyer
	{
		public BaseVm(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public IServiceProvider ServiceProvider { get; }
	}
}
