using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class Control : DrawableGameComponent
	{
		public Control(Game game)
			: base(game)
		{
		}
	}

	public abstract class PolygonBase : Control
	{
		public PolygonBase(Game game)
			: base(game)
		{
		}

		public abstract IReadOnlyList<Vector2> Vertices { get; }

		public Color Color { get; set; } = Color.Red;

		public float thickness = 1;

		public float Thickness
		{
			get => thickness;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				thickness = value;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			((Game1)Game).SpriteBatch.DrawPolygon(Vertices, Color, Thickness);
		}
	}

	public class PlotControl : PolygonBase
	{
		public PlotControl(Game game, SubPlotRect plot)
			: base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			NotifyPropertyExtentions.TrySubscrible(Plot, PlotPropertyChanged);
		}

		public SubPlotRect Plot { get; }

		private void PlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			vertices = null;
		}

		private Vector2[]? vertices;

		public override IReadOnlyList<Vector2> Vertices
		{
			get
			{
				if (vertices is null)
				{
					var bounds = Plot?.Bounds ?? System.Drawing.RectangleF.Empty;
					vertices = new Vector2[]
					{
						new Vector2(bounds.Left, bounds.Top),
						new Vector2(bounds.Right, bounds.Top),
						new Vector2(bounds.Right, bounds.Bottom),
						new Vector2(bounds.Left, bounds.Bottom),
					};
				}

				return vertices;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				NotifyPropertyExtentions.TryUnsubscrible(Plot, PlotPropertyChanged);
				Game.Components.Remove(this);
			}

			base.Dispose(disposing);
		}
	}

	public class GridControl : PlotControl
	{
		public GridControl(Game game, GridPlot grid)
			: base(game, grid)
		{
			Grid = grid ?? throw new ArgumentNullException(nameof(grid));
			Grid.Childs.CollectionChanged += GridChildsCollectionChanged;

			foreach (var child in Grid.Childs)
			{
				Childs.Add(CreateControl(child.Child));
			}
		}

		private void GridChildsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			while (Grid.Childs.Count > Childs.Count)
			{
				var child = Grid.Childs[Childs.Count].Child;

				Childs.Add(CreateControl(child));
			}

			while (Grid.Childs.Count < Childs.Count)
			{
				var lastIndex = Childs.Count - 1;
				Childs[lastIndex].Dispose();
				Childs.RemoveAt(lastIndex);
			}

			for (var i = 0; i < Childs.Count; i++)
			{
				if (!EqualityComparer<SubPlotRect>.Default.Equals(Grid.Childs[i].Child, Childs[i].Plot))
				{
					Childs[i].Dispose();
					Childs[i] = CreateControl(Grid.Childs[i].Child);
				}
			}
		}

		private PlotControl CreateControl(SubPlotRect plot)
		{
			var result = plot is GridPlot gridPlot ? new GridControl(Game, gridPlot) : new PlotControl(Game, plot);
			Game.Components.Add(result);
			return result;
		}

		private List<PlotControl> Childs { get; } = new List<PlotControl>();

		public GridPlot Grid { get; }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Grid.Childs.CollectionChanged -= GridChildsCollectionChanged;
			}

			base.Dispose(disposing);
		}
	}
}
