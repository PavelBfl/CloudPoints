using System;

namespace StepFlow.ViewModel
{
    public interface IContextElement
	{
		IServiceProvider ServiceProvider { get; }

		WrapperProvider WrapperProvider { get; }
	}
}
