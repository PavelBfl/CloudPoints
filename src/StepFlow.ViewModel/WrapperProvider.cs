using System;
using System.Collections.Generic;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class WrapperProvider
	{
		public WrapperProvider(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public IServiceProvider ServiceProvider { get; }

		private Dictionary<object, object> ByModel { get; } = new Dictionary<object, object>();

		public void Add(object model, object viewModel)
		{
			if (viewModel is null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			ByModel.Add(model, viewModel);
		}

		public bool Remove(object model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			return ByModel.Remove(model);
		}

		public bool TryGetViewModel(object model, out object viewModel) => ByModel.TryGetValue(model, out viewModel);

		public object GetViewModel(object model) => ByModel[model];

		public object? GetViewModelOrDefault(object model) => ByModel.GetValueOrDefault(model);

		internal object GetOrCreate(object model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			if (!TryGetViewModel(model, out var result))
			{
				result = model switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommand(this, moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(this, createCommand),
					GamePlay.Node node => new NodeVm(this, node),
					GamePlay.Piece piece => new PieceVm(this, piece),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};
			}

			return result;
		}

		internal T GetOrCreate<T>(object model) => (T)GetOrCreate(model);
	}
}
