using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public abstract class WrapperVm<T> : BaseVm, IWrapper<T>
		where T : class
	{
		public WrapperVm(LockProvider lockProvider, T source)
			: base(lockProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public bool Lock { get; protected set; }

		public virtual IEnumerable<ILockable> GetContent() => Enumerable.Empty<ILockable>();

		public virtual void Dispose()
		{
		}

		public T Source { get; }

		public virtual void SourceHasChange()
		{
		}
	}
}
