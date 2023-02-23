using System;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm
	{
		public WrapperVm(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
		}

		[AllowNull]
		[MaybeNull]
		internal virtual T Source { get; set; }
	}
}
