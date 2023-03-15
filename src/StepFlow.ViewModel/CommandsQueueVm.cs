using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using StepFlow.ViewModel.Commands;
using StepFlow.ViewModel.Exceptions;
using StepFlow.ViewModel.Services;

namespace StepFlow.ViewModel
{
	public class CommandsQueueVm : IList<CommandVm>, INotifyCollectionChanged, IMarkered
	{
		public CommandsQueueVm(PieceVm owner)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			WrapperProvider = Owner.ServiceProvider.GetRequiredService<IWrapperProvider>();
		}

		private IWrapperProvider WrapperProvider { get; }

		public PieceVm Owner { get; }

		private bool isMark;

		public event NotifyCollectionChangedEventHandler? CollectionChanged;

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

		private List<CommandVm> commands = new List<CommandVm>();
		private List<CommandVm> Commands
		{
			get
			{
				if (SourceChange)
				{
					var index = 0;
					foreach (var command in Owner.Source.Commands)
					{
						if (index < commands.Count)
						{
							if (commands[index].Source != command)
							{
								commands[index] = CreateCommandVm(command);
							}
						}
						else if (index == commands.Count)
						{
							commands.Add(CreateCommandVm(command));
						}
						index++;
					}

					while (index < commands.Count)
					{
						var removedIndex = commands.Count - 1;
						commands[removedIndex].Dispose();
						commands.RemoveAt(removedIndex);
					}
					SourceChange = false;
				}

				return commands;
			}
		}

		private CommandVm CreateCommandVm(GamePlay.Command modelCommand)
		{
			return modelCommand switch
			{
				GamePlay.MoveCommand moveCommand => new MoveCommand(
					Owner.ServiceProvider,
					(PieceVm)WrapperProvider.GetViewModel(moveCommand.Target),
					(NodeVm)WrapperProvider.GetViewModel(moveCommand.Next)
				),
				_ => throw new InvalidViewModelException(),
			};
		}

		private bool SourceChange { get; set; }

		private void Refresh()
		{
			SourceChange = true;
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public int Count => Commands.Count;

		public bool IsReadOnly => false;

		public CommandVm this[int index] { get => Commands[index]; set => Commands[index] = value; }

		public int IndexOf(CommandVm item) => Commands.IndexOf(item);

		public void Insert(int index, CommandVm item)
		{
			Owner.Source.Commands.Insert(index, item.Source);
			Refresh();
		}

		public void RemoveAt(int index)
		{
			Owner.Source.Commands.RemoveAt(index);
			Refresh();
		}

		public void Add(CommandVm item)
		{
			Owner.Source.Commands.Add(item.Source);
			Refresh();
		}

		public void Clear()
		{
			Owner.Source.Commands.Clear();
			Refresh();
		}

		public bool Contains(CommandVm item) => Commands.Contains(item);

		public void CopyTo(CommandVm[] array, int arrayIndex) => Commands.CopyTo(array, arrayIndex);

		public bool Remove(CommandVm item)
		{
			var removed = Owner.Source.Commands.Remove(item.Source);
			if (removed)
			{
				Refresh();
			}

			return removed;
		}

		public IEnumerator<CommandVm> GetEnumerator() => Commands.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
