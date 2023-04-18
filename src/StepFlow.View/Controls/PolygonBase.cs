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

		public abstract IReadOnlyVertices Vertices { get; }

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

		public bool Contains(Vector2 point) => Vertices.Bounds.Contains(point.X, point.Y) && Utils.Contains(Vertices, point);

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);


		}

		public override void Draw(GameTime gameTime)
		{
			((Game1)Game).SpriteBatch.DrawPolygon(Vertices, Color, Thickness);
		}
	}
}
