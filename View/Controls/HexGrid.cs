using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Layout;
using StepFlow.ViewModel;

namespace StepFlow.View.Controls
{
	public class HexGrid : Control
	{
		public HexGrid(Game game, WorldVm source, SubPlotRect plot) : base(game)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));

			Childs = new HexChild[Source.ColsCount, Source.RowsCount];
			for (var iCol = 0; iCol < Source.ColsCount; iCol++)
			{
				for (var iRow = 0; iRow < Source.RowsCount; iRow++)
				{
					var child = new HexChild(Game, this, new Point(iCol, iRow));
					Childs[iCol, iRow] = child;
					Game.Components.Add(child);
				}
			}

			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			Plot.PropertyChanged += PlotPropertyChanged;

			Refresh();
		}

		public WorldVm Source { get; }

		private HexChild[,] Childs { get; }

		private SubPlotRect Plot { get; }

		private void PlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(SubPlotRect.Bounds):
					foreach (var child in Childs.Cast<HexChild>())
					{
						child.Clear();
					}
					break;
			}
		}

		private HexOrientation orientation = HexOrientation.Flat;

		public HexOrientation Orientation
		{
			get => orientation;
			set
			{
				if (Orientation != value)
				{
					orientation = value;
					Refresh();
				}
			}
		}

		private bool offsetOdd;

		public bool OffsetOdd
		{
			get => offsetOdd;
			set
			{
				if (OffsetOdd != value)
				{
					offsetOdd = value;
					Refresh();
				}
			}
		}

		public float size = 1;

		public float Size
		{
			get => size;
			set
			{
				if (Size != value)
				{
					size = value;
					Refresh();
				}
			}
		}

		private void Refresh()
		{
			switch (Orientation)
			{
				case HexOrientation.Flat:
					Width = Size * 2;
					Height = MathF.Sqrt(3) * Size;
					CellWidth = Width / 4;
					CellHeight = Height / 2;
					break;
				case HexOrientation.Pointy:
					Width = MathF.Sqrt(3) * Size;
					Height = 3 / 2 * Size;
					CellWidth = Width / 2;
					CellHeight = Height / 4;
					break;
				default: throw EnumNotSupportedException.Create(Orientation);
			}
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
		public float CellWidth { get; private set; }
		public float CellHeight { get; private set; }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Plot.PropertyChanged -= PlotPropertyChanged;
				foreach (var child in Childs.Cast<HexChild>())
				{
					child.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
