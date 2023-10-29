using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public class Texture : Primitive
	{
		public Texture(IServiceProvider serviceProvider) : base(serviceProvider) => Drawer = ServiceProvider.GetRequiredService<IDrawer>();

		private IDrawer Drawer { get; }

		public Texture2D? Texture2D { get; set; }

		public Rectangle Rectangle { get; set; }

		public override void Draw(GameTime gameTime)
		{
			if (Texture2D is not null)
			{
				Drawer.SpriteBatch.Draw(Texture2D, Rectangle, Color.White);
			}

			base.Draw(gameTime);
		}
	}
}
