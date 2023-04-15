using System;
using System.Collections.Generic;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class WrapperProvider
	{
		private Dictionary<object, object> ByViewModel { get; } = new Dictionary<object, object>();

		private Dictionary<object, object> ByModel { get; } = new Dictionary<object, object>();

		public void Add(object viewModel, object model)
		{
			if (viewModel is null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			ByViewModel.Add(viewModel, model);
			ByModel.Add(model, viewModel);
		}

		public void Remove(object viewModel, object model)
		{
			if (viewModel is null)
			{
				throw new ArgumentNullException(nameof(viewModel));
			}

			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			ByViewModel.Remove(viewModel);
			ByModel.Remove(model);
		}

		public bool TryGetModel(object viewModel, out object model) => ByViewModel.TryGetValue(viewModel, out model);

		public bool TryGetViewModel(object model, out object viewModel) => ByModel.TryGetValue(model, out viewModel);

		public object GetModel(object viewModel) => ByViewModel[viewModel];

		public object GetViewModel(object model) => ByModel[model];

		public object? GetModelOrDefault(object viewModel) => ByViewModel.GetValueOrDefault(viewModel);

		public object? GetViewModelOrDefault(object model) => ByModel.GetValueOrDefault(model);

		internal CommandVm GetOrCreateCommand(GamePlay.Commands.Command command)
		{
			if (TryGetViewModel(command, out var result))
			{
				return (CommandVm)result;
			}
			else
			{
				return command switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommand(
						(IContextElement)GetViewModel(command.TargetRequired),
						moveCommand
					),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(
						(IContextElement)GetViewModel(command.TargetRequired),
						createCommand
					),
					_ => throw Exceptions.Builder.CreateUnknownCommand(),
				};
			}
		}
	}
}
