using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public class Polygon : Node
	{
		public Polygon(Game game)
			: base(game)
		{
		}

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
				((Game1)Game).SpriteBatch.DrawPolygon(Vertices, Color, Thickness);
			}
		}
	}
}
