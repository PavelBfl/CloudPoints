using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using StepFlow.Common;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class PlotControl : PolygonBase
	{
		public PlotControl(Game game, RectPlot plot)
			: base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
			NotifyPropertyExtensions.TrySubscribe(Plot, PlotPropertyChanged);
		}

		public RectPlot Plot { get; }

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
				NotifyPropertyExtensions.TryUnsubscribe(Plot, PlotPropertyChanged);
				Game.Components.Remove(this);
			}

			base.Dispose(disposing);
		}
	}
}
