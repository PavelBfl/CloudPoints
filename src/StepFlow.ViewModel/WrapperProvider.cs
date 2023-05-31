using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StepFlow.Core;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class WrapperProvider
	{
		private Dictionary<object, IWrapper> ViewModels { get; } = new Dictionary<object, IWrapper>();

		public bool TryGetValue(object model, out IWrapper result) => ViewModels.TryGetValue(model, out result);

		public bool TryGetValue<T>(object model, [MaybeNullWhen(false)] out T result)
			where T : IWrapper
		{
			if (TryGetValue(model, out IWrapper viewModel))
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
		public IWrapper? Get(object? model) => model is { } ? ViewModels[model] : null;

		[return: NotNullIfNotNull(nameof(model))]
		[return: MaybeNull]
		public T Get<T>(object? model)
			where T : IWrapper
			=> (T)Get(model);

		[return: NotNullIfNotNull(nameof(model))]
		public IWrapper? GetOrCreate(object? model)
		{
			if (model is null)
			{
				return null;
			}

			if (!ViewModels.TryGetValue(model, out var result))
			{
				result = model switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommandVm(this, moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommandVm(this, createCommand),
					GamePlay.Node node => new NodeVm(this, node),
					GamePlay.Piece piece => new PieceVm(this, piece),
					Axis<GamePlay.Commands.Command> axis => new AxisVm(this, axis),
					GamePlay.Context context => new ContextVm(this, context),
					GamePlay.World world => new WorldVm(this, world),
					Place<GamePlay.Node> place => new PlaceVm(this, place),
					PiecesCollection<GamePlay.Piece> pieceCollection => new PiecesCollectionVm(this, pieceCollection),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};

				ViewModels.Add(model, result);
			}

			return result;
		}

		[return: NotNullIfNotNull(nameof(model))]
		[return: MaybeNull]
		public T GetOrCreate<T>(object? model)
			where T : IWrapper
			=> (T)GetOrCreate(model);

		public void Clear()
		{
			var locks = new HashSet<IWrapper>();

			var roots = ViewModels.Values.Where(x => x.Lock).ToArray();

			foreach (var root in roots)
			{
				SetLock(root, locks);
			}

			foreach (var (key, wrapper) in ViewModels.ToArray())
			{
				if (!locks.Contains(wrapper))
				{
					ViewModels.Remove(wrapper);
					wrapper.Dispose();
				}
			}
		}

		private void SetLock(IWrapper current, HashSet<IWrapper> locks)
		{
			if (locks.Add(current))
			{
				foreach (var content in current.GetContent())
				{
					SetLock(content, locks);
				}
			}
		}
	}

	public interface IWrapper : IDisposable
	{
		bool Lock { get; }

		IEnumerable<IWrapper> GetContent();
	}

	public static class WrapperExtensions
	{
		public static IEnumerable<IWrapper> ConcatIfNotNull(this IEnumerable<IWrapper> container, params IWrapper?[] wrappers)
		{
			var wrappersRequired = from wrapper in wrappers
								   where wrapper is { }
								   select wrapper;

			return container.Concat(wrappersRequired);
		}
	}
}
