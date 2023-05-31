using System;
using System.Collections.Generic;
using System.Linq;

namespace StepFlow.ViewModel
{
	public abstract class WrapperVm<T> : BaseVm, IWrapper
		where T : notnull
	{
		public WrapperVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public bool Lock { get; protected set; }

		internal T Source { get; }

		public virtual void Dispose()
		{
		}

		public virtual IEnumerable<IWrapper> GetContent() => Enumerable.Empty<IWrapper>();

		public virtual void Refresh()
		{
		}
	}
}
