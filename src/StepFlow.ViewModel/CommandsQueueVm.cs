using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using StepFlow.TimeLine;
using StepFlow.ViewModel.Exceptions;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : ObservableCollection<ICommandVm>, INotifyCollectionChanged, IMarkered
	{
		public CommandsQueueVm(PieceVm source)
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

		private void RefreshOrder()
		{
			if (this.Any())
			{
				var startTime = Source.Owner.TimeAxis.NearestAllow;

				for (var i = 0; i < Count; i++)
				{
					var command = this[i];
					var newTime = startTime + i;

					if (Source.Owner.TimeAxis.TryGetTime(command, out var time))
					{
						if (newTime != time)
						{
							Source.Owner.TimeAxis.Remove(command);
							Source.Owner.TimeAxis.Registry(newTime, command);
						}
					}
					else
					{
						Source.Owner.TimeAxis.Registry(newTime, command);
					}
				}
			}
		}

		protected override void InsertItem(int index, ICommandVm item)
		{
			var result = WrapItem(item);

			base.InsertItem(index, result);

			RefreshOrder();
		}

		protected override void SetItem(int index, ICommandVm item)
		{
			var result = WrapItem(item);

			base.SetItem(index, result);

			RefreshOrder();
		}

		private PieceVm Source { get; }

		private void ValidateItem(ICommandVm item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (Source != item.Current)
			{
				throw InvalidViewModelException.CreateInvalidSync();
			}
		}

		private ICommandVm WrapItem(ICommandVm item)
		{
			ValidateItem(item);

			return new LocalCommand(this, item);
		}

		protected override void RemoveItem(int index) => RemoveItem(index, true);

		private void RemoveItem(int index, bool removeAxis)
		{
			if (removeAxis)
			{
				Source.Owner.TimeAxis.Remove(this[index]);
			}

			base.RemoveItem(index);

			RefreshOrder();
		}

		protected override void ClearItems()
		{
			foreach (var item in this)
			{
				Source.Owner.TimeAxis.Remove(item);
			}

			base.ClearItems();
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			base.MoveItem(oldIndex, newIndex);

			RefreshOrder();
		}

		private class LocalCommand : CommandWrapper<ICommandVm>, ICommandVm
		{
			public LocalCommand(CommandsQueueVm owner, ICommandVm source) : base(source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CommandsQueueVm Owner { get; }

			public PieceVm? Current => Source.Current;

			public bool IsMark { get => Source.IsMark; set => Source.IsMark = value; }

			public override void Dispose()
			{
				Owner.RemoveItem(Owner.IndexOf(this), false);

				base.Dispose();
			}
		}
	}
}
