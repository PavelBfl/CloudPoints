using System;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm, IDisposable
		where T : notnull
	{
		public WrapperVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			WrapperProvider.Add(Source, this);
		}

		internal T Source { get; }

		public virtual void Dispose() => WrapperProvider.Remove(Source);
	}
}
