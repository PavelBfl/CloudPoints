using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View.Controls
{
	public class Polygon : Primitive
	{
		public Polygon(Game game)
			: base(game)
		{
			Drawer = Game.Services.GetRequiredService<IDrawer>();
		}

		private IDrawer Drawer { get; }

		public IReadOnlyVertices? Vertices { get; set; }

		public Color Color { get; set; } = Color.Red;

		public float thickness = 1;

		public float Thickness
		{
			get => thickness;
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				thickness = value;
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (Vertices?.Any() ?? false)
			{
				Drawer.Polygon(Vertices, Color, Thickness);
			}
		}
	}
}
