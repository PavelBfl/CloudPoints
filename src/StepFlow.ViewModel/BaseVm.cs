using System;
using StepFlow.Common;

namespace StepFlow.ViewModel
{
    public class BaseVm : BaseNotifyer, IContextElement
	{
		public BaseVm(IContextElement context)
		{
			ServiceProvider = context.ServiceProvider;
			WrapperProvider = context.WrapperProvider;
		}

		public IServiceProvider ServiceProvider { get; }

		public WrapperProvider WrapperProvider { get; }
	}
}
