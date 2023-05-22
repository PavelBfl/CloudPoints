using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.ViewModel
{
	public abstract class WrapperVm<T> : BaseVm, IDisposable
		where T : notnull
	{
		public WrapperVm(T source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		internal T Source { get; }

		public virtual void Refresh()
		{
		}
	}
}
