using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using StepFlow.Common;
using StepFlow.Layout;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel.Layout
{
	public class RootVm
	{
		public RootVm(ContextVm context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			NotifyPropertyExtensions.TrySubscribe(Context, ContextPropertyChanging, ContextPropertyChanged);

			Root = new GridPlot()
			{
				Rows =
				{
					new CellSize(1, UnitMeasure.Ptc),
					new CellSize(100, UnitMeasure.Pixels),
				},
				Columns =
				{
					new CellSize(1, UnitMeasure.Ptc),
				}
			};

			ActionPlot = new RectPlot()
			{
				Margin = new Margin(5),
			};

			QueueCommandsContainer = new GridPlot()
			{
				Margin = new Margin(0),
				Rows =
				{
					new CellSize(1, UnitMeasure.Ptc),
				}
			};

			Root.Childs.Add(ActionPlot, new CellPosition(0, 0));
			Root.Childs.Add(QueueCommandsContainer, new CellPosition(0, 1));

			RefreshQueue(Array.Empty<CommandVm>());
		}

		private void ContextPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ContextVm.Current):
					NotifyPropertyExtensions.TrySubscribe(Context.Current?.Commands, CommandsCollectionChanged);
					break;
			}
		}

		private void ContextPropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(ContextVm.Current):
					NotifyPropertyExtensions.TryUnsubscribe(Context.Current?.Commands, CommandsCollectionChanged);
					break;
			}
		}

		private void CommandsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshQueue((IReadOnlyList<CommandVm>?)Context.Current?.Commands ?? Array.Empty<CommandVm>());
		}

		private void RefreshQueue(IReadOnlyList<CommandVm> commandsQueue)
		{
			while (commandsQueue.Count > QueueCommandInner.Count)
			{
				QueueCommandsContainer.Columns.Add(new CellSize(QueueCommandsContainer.Bounds.Height, UnitMeasure.Pixels));

				var newCommand = new CommandLayout()
				{
					Margin = new Margin(5),
					Size = new System.Drawing.SizeF(50, 50),
				};
				QueueCommandsContainer.Childs.Add(newCommand, new CellPosition(QueueCommandsContainer.Columns.Count - 1, 0));
				QueueCommandInner.Add(newCommand);
			}

			while (commandsQueue.Count < QueueCommandInner.Count)
			{
				QueueCommandsContainer.Childs.Remove(QueueCommandInner[QueueCommandInner.Count - 1]);
				QueueCommandInner.RemoveAt(QueueCommandInner.Count - 1);
				QueueCommandsContainer.Columns.RemoveAt(QueueCommandsContainer.Columns.Count - 1);
			}

			for (var i = 0; i < commandsQueue.Count; i++)
			{
				QueueCommandInner[i].Command = commandsQueue[i];
			}
		}

		public ContextVm Context { get; }

		public GridPlot Root { get; }

		public RectPlot ActionPlot { get; }

		public GridPlot QueueCommandsContainer { get; }

		private ObservableCollection<CommandLayout> QueueCommandInner { get; } = new ObservableCollection<CommandLayout>();

		public IReadOnlyList<CommandLayout> QueueCommands => QueueCommandInner;
	}
}
