using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using StepFlow.TimeLine;

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

		public ICommandVm Registry(ICommandVm command)
		{
			ValidateItem(command);

			var result = new LocalCommand(this, command);
			Add(result);
			return result;
		}

		private IPieceVm Source { get; }

		private void ValidateItem(ICommandVm item)
		{
			if (item is null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (Source != item.Current)
			{
				// TODO Переделать на константу
				throw new InvalidOperationException("Failed sync command with piece");
			}
		}

		private ICommandVm WrapItem(ICommandVm item)
		{
			ValidateItem(item);

			return new LocalCommand(this, item);
		}

		private class LocalCommand : CommandWrapper<ICommandVm>, ICommandVm
		{
			public LocalCommand(CommandsQueueVm owner, ICommandVm source) : base(source)
			{
				Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			}

			public CommandsQueueVm Owner { get; }

			public IPieceVm? Current => Source.Current;

			public bool IsMark { get => Source.IsMark; set => Source.IsMark = value; }

			public override void Dispose()
			{
				Owner.Remove(this);

				base.Dispose();
			}
		}
	}
}
