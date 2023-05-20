using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StepFlow.ViewModel
{
	public interface IWrapperBaseVm
	{
		bool IsUse { get; set; }

		IEnumerable<IWrapperBaseVm> GetContent();

		void Refresh();
	}

	public abstract class WrapperVm<T> : BaseVm, IWrapperBaseVm, IDisposable
		where T : notnull
	{
		public WrapperVm(WrapperProvider wrapperProvider, T source)
			: base(wrapperProvider)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public bool IsUse { get; set; }

		internal T Source { get; }

		public virtual void Dispose()
		{
		}

		public abstract IEnumerable<IWrapperBaseVm> GetContent();

		public virtual void Refresh()
		{
		}

		private sealed class Accessor<TModel, TViewModel> : IAccessorVm<TModel, TViewModel>
			where TModel : notnull
			where TViewModel : IWrapperBaseVm
		{
			public Accessor(WrapperProvider provider, TModel model)
			{
				Provider = provider ?? throw new ArgumentNullException(nameof(provider));
				this.model = model ?? throw new ArgumentNullException(nameof(model));
			}

			private WrapperProvider Provider { get; }

			private TModel model;

			public TModel Model
			{
				get => model;
				set
				{
					if (!EqualityComparer<TModel>.Default.Equals(Model, value))
					{
						model = value;
						viewModel = default;
					}
				}
			}

			[AllowNull]
			[MaybeNull]
			private TViewModel viewModel;

			public TViewModel ViewModel => viewModel ??= Provider.GetOrCreate<TViewModel>(Model);
		}

		private sealed class AccessorAllowNull<TModel, TViewModel> : IAccessorVm<TModel, TViewModel>
			where TViewModel : IWrapperBaseVm
		{
			public AccessorAllowNull(WrapperProvider provider, TModel model)
			{
				Provider = provider ?? throw new ArgumentNullException(nameof(model));
				this.model = model;
			}

			private WrapperProvider Provider { get; }

			private bool IsChange { get; set; } = true;

			private TModel model;

			public TModel Model
			{
				get => model;
				set
				{
					if (!EqualityComparer<TModel>.Default.Equals(Model, value))
					{
						model = value;
						IsChange = true;
					}
				}
			}

			[AllowNull]
			[MaybeNull]
			private TViewModel viewModel;

			public TViewModel ViewModel
			{
				get
				{
					if (IsChange)
					{
						if (Model is null)
						{
							viewModel = default;
						}
						else
						{
							viewModel = Provider.GetOrCreate<TViewModel>(Model);
						}

						IsChange = false;
					}

					return viewModel;
				}
			}
		}
	}

	public interface IAccessorVm<TModel, TViewModel>
	{
		TModel Model { get; set; }

		TViewModel ViewModel { get; }
	}
}
