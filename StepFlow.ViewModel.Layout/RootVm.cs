using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Layout;

namespace StepFlow.ViewModel.Layout
{
	public class RootVm : IDisposable
	{
		public RootVm(int colsCount, int rowsCount, HexOrientation orientation, bool offsetOdd)
			: this(new WorldVm(new World(colsCount, rowsCount, orientation, offsetOdd)))
		{
		}

		public RootVm(WorldVm world)
		{
			World = world ?? throw new ArgumentNullException(nameof(world));
			NotifyPropertyExtentions.TrySubscrible(World, WorldPropertyChanging, WorldPropertyChanged);

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
		}

		private void WorldPropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(WorldVm.Current):
					NotifyPropertyExtentions.TryUnsubscrible(World.Current?.CommandQueue, CommandQueueCollectionChanged);
					break;
			}
		}

		private void WorldPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(WorldVm.Current):
					NotifyPropertyExtentions.TrySubscrible(World.Current?.CommandQueue, CommandQueueCollectionChanged);
					break;
			}
		}

		private void CommandQueueCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var commandQueue = World.Current.CommandQueue;

			while (commandQueue.Count > QueueCommandInner.Count)
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

			while (commandQueue.Count < QueueCommandInner.Count)
			{
				QueueCommandsContainer.Childs.Remove(QueueCommandInner[QueueCommandInner.Count - 1]);
				QueueCommandInner.RemoveAt(QueueCommandInner.Count - 1);
				QueueCommandsContainer.Columns.RemoveAt(QueueCommandsContainer.Columns.Count - 1);
			}

			for (var i = 0; i < commandQueue.Count; i++)
			{
				QueueCommandInner[i].Command = commandQueue[i];
			}
		}

		public WorldVm World { get; }

		public GridPlot Root { get; }

		public RectPlot ActionPlot { get; }

		public GridPlot QueueCommandsContainer { get; }

		private ObservableCollection<CommandLayout> QueueCommandInner { get; } = new ObservableCollection<CommandLayout>();

		public IReadOnlyList<CommandLayout> QueueCommands => QueueCommandInner;

		public void Dispose()
		{
			NotifyPropertyExtentions.TryUnsubscrible(World, WorldPropertyChanging, WorldPropertyChanged);
			NotifyPropertyExtentions.TryUnsubscrible(World.Current?.CommandQueue, CommandQueueCollectionChanged);
		}
	}
}
