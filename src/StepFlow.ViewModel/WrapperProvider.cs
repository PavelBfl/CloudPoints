using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StepFlow.Core;
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

		[return: NotNullIfNotNull(nameof(model))]
		public object? Get(object? model) => model is { } ? ViewModels[model] : null;

		[return: NotNullIfNotNull(nameof(model))]
		[return: MaybeNull]
		public T Get<T>(object? model) => (T)Get(model);

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
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommandVm(this, moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(this, createCommand),
					GamePlay.Node node => new NodeVm(this, node),
					GamePlay.Piece piece => new PieceVm(this, piece),
					Axis<GamePlay.Commands.Command> axis => new AxisVm(this, axis),
					GamePlay.Context context => new ContextVm(this, context),
					GamePlay.World world => new WorldVm(this, world),
					Place<GamePlay.Node> place => new PlaceVm(this, place),
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
