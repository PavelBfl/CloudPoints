using System;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.ViewModel.Services;

namespace StepFlow.ViewModel
{
	public class WrapperVm<T> : BaseVm, IDisposable
		where T : notnull
	{
		public WrapperVm(IServiceProvider serviceProvider, T source)
			: base(serviceProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			WrapperProvider = ServiceProvider.GetRequiredService<IWrapperProvider>();

			WrapperProvider.Add(this, Source);
		}

		internal T Source { get; }

		protected IWrapperProvider WrapperProvider { get; }

		public virtual void Dispose() => WrapperProvider.Remove(this, Source);
	}
}
