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

		public Color Color { get; set; } = Color.Black;

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
		public PlotControl(Game game)
			: base(game)
		{
		}

		private SubPlotRect? plot;

		public SubPlotRect? Plot
		{
			get => plot;
			set
			{
				if (Plot != value)
				{
					NotifyPropertyExtentions.TryUnsubscrible(Plot, PlotPropertyChanged);
					plot = value;
					NotifyPropertyExtentions.TrySubscrible(Plot, PlotPropertyChanged);
				}
			}
		}

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
	}

	public class GridControl : PlotControl
	{
		public GridControl(Game game, GridPlot grid)
			: base(game)
		{
			Grid = grid ?? throw new ArgumentNullException(nameof(grid));
			Grid.Childs.CollectionChanged += GridChildsCollectionChanged;
		}

		private void GridChildsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			while (Grid.Childs.Count > Childs.Count)
			{

			}
		}

		private List<PlotControl> Childs { get; } = new List<PlotControl>();

		public GridPlot Grid { get; }
	}
}
