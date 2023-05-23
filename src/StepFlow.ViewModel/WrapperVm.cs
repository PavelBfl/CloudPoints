using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.ViewModel
{
	public abstract class WrapperVm<T> : BaseVm
		where T : notnull
	{
		public WrapperVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		internal T Source { get; }

		public virtual void Refresh()
		{
		}
	}
}
