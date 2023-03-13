using System.Collections.Generic;

namespace StepFlow.ViewModel.Services
{
	public interface IWrapperProvider
	{
		void Add(object viewModel, object model);

		void Remove(object viewModel, object model);
		bool TryGetModel(object viewModel, out object model);
		bool TryGetViewModel(object model, out object viewModel);

		object GetModel(object viewModel) => TryGetModel(viewModel, out var result) ? result : throw new KeyNotFoundException();

		object GetViewModel(object model) => TryGetViewModel(model, out var result) ? result : throw new KeyNotFoundException();

		object? GetModelOrDefault(object viewModel) => TryGetModel(viewModel, out var result) ? result : null;

		object? GetViewModelOrDefault(object model) => TryGetViewModel(model, out var result) ? result : null;
	}
}
