using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Core;
using StepFlow.Layout;
using StepFlow.ViewModel;

namespace StepFlow.View.Controls
{
	public class HexGrid : Control
	{
		private static float BigRadiusToFlatRatio { get; } = MathF.Sqrt(3);
		private static (float Pointy, float Flat, float CellPointy, float CellFlat) GetSize(float bigRadius)
		{
			var pointy = bigRadius * 2;
			var flat = bigRadius * BigRadiusToFlatRatio;
			return (pointy, flat, pointy / 4, flat / 2);
		}

		public HexGrid(Game game, WorldVm source, SubPlotRect plot) : base(game)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));

			Childs = new HexChild[Source.ColsCount, Source.RowsCount];
			for (var iCol = 0; iCol < Source.ColsCount; iCol++)
			{
				for (var iRow = 0; iRow < Source.RowsCount; iRow++)
				{
					var child = new HexChild(Game, this, Source[iCol, iRow]);
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
					(Width, Height, CellWidth, CellHeight) = GetSize(Size);
					break;
				case HexOrientation.Pointy:
					(Height, Width, CellHeight, CellWidth) = GetSize(Size);
					break;
				default: throw EnumNotSupportedException.Create(Orientation);
			}
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
		public float CellWidth { get; private set; }
		public float CellHeight { get; private set; }

		internal Vector2 GetPosition(Point cellPosition) => new Vector2(
			cellPosition.X * CellWidth + Plot.Bounds.Left + Width / 2,
			cellPosition.Y * CellHeight + Plot.Bounds.Top + Height / 2
		);

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
