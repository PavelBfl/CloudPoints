using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : ObservableCollection<ICommandVm>, INotifyCollectionChanged, ISelectable
	{
		public CommandsQueueVm(IPieceVm source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		private bool isSelected;
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (IsSelected != value)
				{
					isSelected = value;

					foreach (var command in this)
					{
						command.IsSelected = IsSelected;
					}
				}
			}
		}

		private IPieceVm Source { get; }

		private void ValidateItem(ICommandVm item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (Source.Current != item)
			{
				// TODO Переделать на константу
				throw new InvalidOperationException("Failed sync command with piece");
			}
		}

		protected override void InsertItem(int index, ICommandVm item)
		{
			ValidateItem(item);

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, ICommandVm item)
		{
			ValidateItem(item);

			base.SetItem(index, item);
		}
	}
}
