using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.Master;
using StepFlow.ViewModel.Collector;

namespace StepFlow.ViewModel
{
	public interface IWrapper<out T> : ILockable
		where T : class
	{
		T Source { get; }

		void SourceHasChange();
	}

	public abstract class WrapperVm<T> : BaseVm, IWrapper<T>
		where T : class
	{
		public WrapperVm(LockProvider wrapperProvider, T source)
			: base(wrapperProvider)
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

	public sealed class PlayMasterVm : WrapperVm<PlayMaster>
	{
		public PlayMasterVm(LockProvider wrapperProvider, PlayMaster source) : base(wrapperProvider, source)
		{
		}
	}
}
