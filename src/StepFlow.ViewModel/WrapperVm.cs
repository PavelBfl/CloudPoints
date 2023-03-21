using System;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm, IDisposable
		where T : notnull
	{
		public WrapperVm(IContextElement context, T source)
			: base(context)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			WrapperProvider.Add(this, Source);
		}

		internal T Source { get; }

		public virtual void Dispose() => WrapperProvider.Remove(this, Source);
	}
}
