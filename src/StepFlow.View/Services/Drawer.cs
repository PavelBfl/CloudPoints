using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StepFlow.View.Services
{
	public sealed class Drawer : IDrawer
	{
		public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
		{
			SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

			Pixel = new(graphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });
		}

		private Texture2D Pixel { get; }

		public SpriteBatch SpriteBatch { get; }

		public GraphicsDevice GraphicsDevice { get; }

		public void Line(Vector2 start, Vector2 end, Color color, float thickness = 2f)
		{
			if (thickness < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(thickness));
			}

			Vector2 delta = end - start;
			SpriteBatch.Draw(
				Pixel,
				start,
				null,
				color,
				Utils.ToAngle(delta),
				new Vector2(0, 0.5f),
				new Vector2(delta.Length(), thickness),
				SpriteEffects.None,
				0f
			);
		}

		public void Polygon(IReadOnlyList<Vector2> vertices, Color color, float thickness = 2f)
		{
			if (vertices is null)
			{
				throw new ArgumentNullException(nameof(vertices));
			}

			if (thickness < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(thickness));
			}

			var prevIndex = vertices.Count - 1;
			for (var i = 0; i < vertices.Count; i++)
			{
				Line(vertices[prevIndex], vertices[i], color, thickness);
				prevIndex = i;
			}
		}
	}
}
