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
				Childs[lastIndex].Free();
				Childs.RemoveAt(lastIndex);
			}

			for (var i = 0; i < Childs.Count; i++)
			{
				if (!EqualityComparer<RectPlot>.Default.Equals(Grid.Childs[i].Child, ((PlotControl)Childs[i]).Plot))
				{
					Childs[i].Free();
					Childs[i] = CreateControl(Grid.Childs[i].Child);
				}
			}
		}

		// TODO В будущем этот метод будет заменён на общий хеш
		private PlotControl CreateControl(RectPlot plot)
		{
			var result = plot is GridPlot gridPlot ? new GridControl(Game, gridPlot) : new PlotControl(Game, plot);
			return result;
		}

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
