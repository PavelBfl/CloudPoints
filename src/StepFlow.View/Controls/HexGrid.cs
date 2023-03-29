using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
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

		public HexGrid(Game game, ContextVm source, RectPlot plot) : base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			Plot.PropertyChanged += PlotPropertyChanged;

			Source = source ?? throw new ArgumentNullException(nameof(source));

			Refresh();
		}

		private ContextVm? source;

		public ContextVm? Source
		{
			get => source;
			set
			{
				if (Source != value)
				{
					NotifyPropertyExtensions.TryUnsubscrible(Source?.Place, ParticlesCollectionChanged);

					source = value;

					NotifyPropertyExtensions.TrySubscrible(Source?.Place, ParticlesCollectionChanged);
				}
			}
		}

		private void ParticlesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			var nodesVm = Source?.Place.ToArray();

			if (nodesVm is null || !nodesVm.Any())
			{
				foreach (var child in Childs)
				{
					Game.Components.Remove(child);
					child.Dispose();
				}
				Childs.Clear();
				return;
			}

			while (Childs.Count < nodesVm.Length)
			{
				var nodeV = new HexChild(Game, this);
				Childs.Add(nodeV);
				Game.Components.Add(nodeV);
			}

			while (Childs.Count > nodesVm.Length)
			{
				var lastIndex = Childs.Count - 1;
				var lastNodeV = Childs[lastIndex];
				Childs.RemoveAt(lastIndex);
				Game.Components.Remove(lastNodeV);
				lastNodeV.Dispose();
			}

			for (var i = 0; i < Childs.Count; i++)
			{
				Childs[i].Source = nodesVm[i];
			}
		}

		private List<HexChild> Childs { get; } = new List<HexChild>();

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
			foreach (var child in Childs)
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
				// TODO Restore saving
			}

			base.Update(gameTime);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Plot.PropertyChanged -= PlotPropertyChanged;
				foreach (var child in Childs)
				{
					child.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
