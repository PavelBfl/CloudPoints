using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Layout;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public class PlotControl : Primitive
	{
		public PlotControl(Game game, RectPlot plot)
			: base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			NotifyPropertyExtensions.TrySubscribe(Plot, PlotPropertyChanged);

			Bounds = new Polygon(Game)
			{
				Vertices = Vertices,
			};

			BoundsRefresh();
		}

		private Polygon Bounds { get; }

		public RectPlot Plot { get; }

		private void PlotPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(RectPlot.Bounds))
			{
				BoundsRefresh();
			}
		}

		private void BoundsRefresh() => vertices.Bounds = Plot.Bounds;

		private BoundsVertices vertices = new BoundsVertices();

		public IReadOnlyVertices Vertices => vertices;

		public override void Draw(GameTime gameTime)
		{
			Bounds.Draw(gameTime);
			base.Draw(gameTime);
		}

		public override void Free()
		{
			NotifyPropertyExtensions.TryUnsubscribe(Plot, PlotPropertyChanged);
			base.Free();
		}
	}
}
