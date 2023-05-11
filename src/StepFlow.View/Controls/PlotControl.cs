using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class PlotControl : Node
	{
		public PlotControl(Game game, RectPlot plot)
			: base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			NotifyPropertyExtensions.TrySubscribe(Plot, PlotPropertyChanged);

			Childs.Add(new Polygon(Game)
			{
				Vertices = Vertices,
			});

			BoundsRefresh();
		}

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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				NotifyPropertyExtensions.TryUnsubscribe(Plot, PlotPropertyChanged);
			}

			base.Dispose(disposing);
		}
	}
}
