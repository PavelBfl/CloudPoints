using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StepFlow.Core;
using StepFlow.Core.Commands;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel.Collector
{
	public class LockProvider
	{
		private Dictionary<object, ILockable> ViewModels { get; } = new Dictionary<object, ILockable>();

		public bool TryGetValue(object model, out ILockable result) => ViewModels.TryGetValue(model, out result);

		public bool TryGetValue<T>(object model, [MaybeNullWhen(false)] out T result)
			where T : ILockable
		{
			if (TryGetValue(model, out ILockable viewModel))
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
		public ILockable? Get(object? model) => model is { } ? ViewModels[model] : null;

		[return: NotNullIfNotNull(nameof(model))]
		[return: MaybeNull]
		public T Get<T>(object? model)
			where T : ILockable?
			=> (T)Get(model);

		[return: NotNullIfNotNull(nameof(model))]
		public ILockable? GetOrCreate(object? model)
		{
			if (model is null)
			{
				return null;
			}

			if (!ViewModels.TryGetValue(model, out var result))
			{
				result = model switch
				{
					MoveCommand moveCommand => new MoveCommandVm(this, moveCommand),
					CreateCommand createCommand => new CreateCommandVm(this, createCommand),
					Node node => new NodeVm(this, node),
					Piece piece => new PieceVm(this, piece),
					Axis<ICommand> axis => new AxisVm(this, axis),
					Context context => new ContextVm(this, context),
					Playground playground => new PlaygroundVm(this, playground),
					Place place => new PlaceVm(this, place),
					PiecesCollection pieceCollection => new PiecesCollectionVm(this, pieceCollection),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};

				ViewModels.Add(model, result);
			}

			return result;
		}

		[return: NotNullIfNotNull(nameof(model))]
		[return: MaybeNull]
		public T GetOrCreate<T>(object? model)
			where T : ILockable?
			=> (T)GetOrCreate(model);

		public void Clear()
		{
			var locks = new HashSet<ILockable>();

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

		private void SetLock(ILockable current, HashSet<ILockable> locks)
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
}
