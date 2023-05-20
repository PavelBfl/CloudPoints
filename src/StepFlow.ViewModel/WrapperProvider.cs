using System;
using System.Collections.Generic;
using System.Linq;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public class WrapperProvider
	{
		private Dictionary<object, IWrapperBaseVm> ByModel { get; } = new Dictionary<object, IWrapperBaseVm>();

		public void Refresh(IEnumerable<IWrapperBaseVm> roots)
		{
			if (roots is null)
			{
				throw new ArgumentNullException(nameof(roots));
			}

			foreach (var wrapper in ByModel.Values)
			{
				wrapper.IsUse = false;
			}

			foreach (var root in roots)
			{
				SetUse(root);
			}

			foreach (var (key, value) in ByModel.ToArray())
			{
				if (!value.IsUse)
				{
					ByModel.Remove(key);
				}
			}
		}

		public void SetUse(IWrapperBaseVm wrapper)
		{
			wrapper.IsUse = true;

			foreach (var item in wrapper.GetContent())
			{
				if (!item.IsUse)
				{
					SetUse(item);
				}
			}
		}

		public void Add(object model, IWrapperBaseVm viewModel)
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

		public bool TryGetViewModel(object model, out IWrapperBaseVm viewModel) => ByModel.TryGetValue(model, out viewModel);

		public IWrapperBaseVm GetViewModel(object model) => ByModel[model];

		public IWrapperBaseVm? GetViewModelOrDefault(object model) => ByModel.GetValueOrDefault(model);

		internal IWrapperBaseVm GetOrCreate(object model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			if (!TryGetViewModel(model, out var result))
			{
				result = model switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommandVm(this, moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(this, createCommand),
					GamePlay.Node node => new NodeVm(this, node),
					GamePlay.Piece piece => new PieceVm(this, piece),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};
			}

			return result;
		}

		internal T GetOrCreate<T>(object model)
			where T : notnull, IWrapperBaseVm
			=> (T)GetOrCreate(model);
	}
}
