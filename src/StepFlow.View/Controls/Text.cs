using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Layout;

namespace StepFlow.View.Controls
{
	public class Text : Control
	{
		public Text(Game game, RectPlot plot) : base(game)
		{
			Plot = plot ?? throw new ArgumentNullException(nameof(plot));
		}

		private RectPlot Plot { get; }

		public string? Content { get; set; }

		public Color Color { get; set; }

		public SpriteFont? Font { get; set; }

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!string.IsNullOrWhiteSpace(Content) && Font is { })
			{
				((Game1)Game).SpriteBatch.DrawString(
					Font,
					Content,
					new Vector2(Plot.Bounds.Top, Plot.Bounds.Left),
					Color
				);
			}
		}
	}
}
