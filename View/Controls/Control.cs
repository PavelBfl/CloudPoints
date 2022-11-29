using System;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using StepFlow.View.Layout;

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

		public override void Draw(GameTime gameTime)
		{
			var bounds = BoundProvider.Bounds;
			((Game1)Game).SpriteBatch.DrawPolygon(
				new[]
				{
					new Vector2(bounds.Left, bounds.Top),
					new Vector2(bounds.Right, bounds.Top),
					new Vector2(bounds.Right, bounds.Bottom),
					new Vector2(bounds.Left, bounds.Bottom),
				},
				Color.Red
			);
		}
	}
}
