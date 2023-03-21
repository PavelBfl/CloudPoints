using System;

namespace StepFlow.ViewModel
{
    public struct ContextElement : IContextElement
	{
		public ContextElement(IServiceProvider serviceProvider, WrapperProvider wrapperProvider)
		{
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			WrapperProvider = wrapperProvider ?? throw new ArgumentNullException(nameof(wrapperProvider));
		}

		public IServiceProvider ServiceProvider { get; set; }

		public WrapperProvider WrapperProvider { get; set; }
	}
}
