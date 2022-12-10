using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
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
