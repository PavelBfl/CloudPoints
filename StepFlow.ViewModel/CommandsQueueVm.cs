using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : ObservableCollection<ICommandVm>, INotifyCollectionChanged, IMarkered
	{
		public CommandsQueueVm(IPieceVm source)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
		}

		private bool isMark;
		public bool IsMark
		{
			get => isMark;
			set
			{
				if (IsMark != value)
				{
					isMark = value;

					foreach (var command in this)
					{
						command.IsMark = IsMark;
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
