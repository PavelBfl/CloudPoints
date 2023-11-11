using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public class TextureLayout : LayoutControl
	{
		public TextureLayout(IServiceProvider serviceProvider) : base(serviceProvider) => Drawer = ServiceProvider.GetRequiredService<IDrawer>();

		private IDrawer Drawer { get; }

		public string? Name { get; set; }

		public override void Draw(GameTime gameTime)
		{
			if (Name is not null && Layout is not null)
			{
				var rectangle = new Rectangle(
					(int)Layout.Place.X,
					(int)Layout.Place.Y,
					(int)Layout.Place.Width,
					(int)Layout.Place.Height
				);
				Drawer.Draw(Name, rectangle);
			}

			base.Draw(gameTime);
		}
	}
}
