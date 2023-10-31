using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using StepFlow.Common.Exceptions;
using StepFlow.View.Services;

namespace StepFlow.View.Controls
{
	public class Text : LayoutControl
	{
		public Text(IServiceProvider serviceProvider) : base(serviceProvider) => Drawer = ServiceProvider.GetRequiredService<IDrawer>();

		private IDrawer Drawer { get; }

		public string? Content { get; set; }

		public Color Color { get; set; }

		public HorizontalAlign HorizontalAlign { get; set; }

		public VerticalAlign VerticalAlign { get; set; }

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!string.IsNullOrWhiteSpace(Content) && Layout?.Place is { } place)
			{
				Drawer.DrawString(
					Content,
					place,
					HorizontalAlign,
					VerticalAlign,
					Color
				);
			}
		}
	}
}
