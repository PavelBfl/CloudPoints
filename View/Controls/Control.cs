using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class Control : DrawableGameComponent, INotifyCollectionChanged
	{
		public Control(Game game, SubPlotRect boundProvider)
			: base(game)
		{
			BoundProvider = boundProvider ?? throw new ArgumentNullException(nameof(boundProvider));
		}

		public SubPlotRect BoundProvider { get; }

		public event NotifyCollectionChangedEventHandler? CollectionChanged;
	}

	public abstract class PolygonBase : Control
	{
		public PolygonBase(Game game, SubPlotRect boundProvider)
			: base(game, boundProvider)
		{
			BoundProvider.PropertyChanged += BoundProviderPropertyChanged;
		}

		private void BoundProviderPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SubPlotRect.Bounds))
			{
				vertices = null;
			}
		}

		private Vector2[]? vertices = null;

		public IReadOnlyList<Vector2> Vertices => vertices ??= CreateVertices().ToArray();

		protected abstract IEnumerable<Vector2> CreateVertices();

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

	public class Rect : PolygonBase
	{
		public Rect(Game game, SubPlotRect boundProvider)
			: base(game, boundProvider)
		{
		}

		protected override IEnumerable<Vector2> CreateVertices()
		{
			var bounds = BoundProvider.Bounds;
			yield return new Vector2(bounds.Left, bounds.Top);
			yield return new Vector2(bounds.Right, bounds.Top);
			yield return new Vector2(bounds.Right, bounds.Bottom);
			yield return new Vector2(bounds.Left, bounds.Bottom);
		}
	}

	public class GridRect : Rect
	{
		public GridRect(Game game, GridPlot grid)
			: base(game, grid)
		{
			Grid = grid ?? throw new ArgumentNullException(nameof(grid));
		}

		public GridPlot Grid { get; }
	}
}
