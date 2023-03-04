using System;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm
	{
		public WrapperVm(IServiceProvider serviceProvider, T source)
			: base(serviceProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		internal T Source { get; }
	}
}
