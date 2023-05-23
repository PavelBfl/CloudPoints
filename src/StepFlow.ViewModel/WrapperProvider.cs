using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class WrapperProvider
	{
		private Dictionary<object, object> ViewModels { get; } = new Dictionary<object, object>();

		public bool TryGetValue(object model, out object result) => ViewModels.TryGetValue(model, out result);

		public bool TryGetValue<T>(object model, [MaybeNullWhen(false)] out T result)
		{
			if (TryGetValue(model, out object viewModel))
			{
				result = (T)viewModel;
				return true;
			}
			else
			{
				result = default;
				return false;
			}
		}

		public object GetOrCreate(object model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			if (!ViewModels.TryGetValue(model, out var result))
			{
				result = model switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommandVm(moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(createCommand),
					GamePlay.Node node => new NodeVm(node),
					GamePlay.Piece piece => new PieceVm(piece),
					Axis<GamePlay.Commands.Command> axis => new AxisVm(axis),
					GamePlay.Context context => new ContextVm(context),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};

				ViewModels.Add(model, result);
			}

			return result;
		}

		public T GetOrCreate<T>(object model)
			where T : notnull
			=> (T)GetOrCreate(model);
	}
}
