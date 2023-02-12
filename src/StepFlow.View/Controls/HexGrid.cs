using System;
using System.Collections.Generic;
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

		public HexGrid(Game game, WorldVm source, RectPlot plot) : base(game)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));

			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			Plot.PropertyChanged += PlotPropertyChanged;

			foreach (var node in Source.Nodes.Values)
			{
				var child = new HexChild(Game, this, node);
				Childs.Add(node.Position, child);
				Game.Components.Add(child);
			}

			Refresh();
		}

		public WorldVm Source { get; }

		private Dictionary<System.Drawing.Point, HexChild> Childs { get; } = new Dictionary<System.Drawing.Point, HexChild>();

		private RectPlot Plot { get; }

		private void PlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(RectPlot.Bounds):
					ChildsClear();
					break;
			}
		}

		private void ChildsClear()
		{
			foreach (var child in Childs.Values)
			{
				child.Clear();
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

			ChildsClear();
		}

		public float Width { get; private set; }
		public float Height { get; private set; }
		public float CellWidth { get; private set; }
		public float CellHeight { get; private set; }

		internal Vector2 GetPosition(Point cellPosition) => new Vector2(
			cellPosition.X * CellWidth + Plot.Bounds.Left + Width / 2,
			cellPosition.Y * CellHeight + Plot.Bounds.Top + Height / 2
		);

		public override void Update(GameTime gameTime)
		{
			var game = (Game1)Game;

			if (game.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				Source.TimeAxis.MoveNext();
				Source.TakeStep();
			}
			else if (game.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.F5))
			{
				Source.Save();
			}

			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Plot.PropertyChanged -= PlotPropertyChanged;
				foreach (var child in Childs.Values)
				{
					child.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
