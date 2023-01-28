using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StepFlow.View.Controls
{
	public abstract class PolygonBase : Control
	{
		public PolygonBase(Game game)
			: base(game)
		{
		}

		public abstract IReadOnlyList<Vector2> Vertices { get; }

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
			((Game1)Game).SpriteBatch.DrawPolygon(Vertices, Color, Thickness);
		}
	}
}
