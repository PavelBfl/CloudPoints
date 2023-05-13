using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StepFlow.View
{
	[Obsolete]
	public static class SpriteBatchExtensions
	{
		private static Texture2D? Pixel { get; set; }

		private static Texture2D GetPixel(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice is null)
			{
				throw new ArgumentNullException(nameof(graphicsDevice));
			}

			if (Pixel is null)
			{
				Pixel = new(graphicsDevice, 1, 1);
				Pixel.SetData(new[] { Color.White });
			}

			return Pixel;
		}

		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 2f)
		{
			if (spriteBatch is null)
			{
				throw new ArgumentNullException(nameof(spriteBatch));
			}

			if (thickness < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(thickness));
			}

			Vector2 delta = end - start;
			spriteBatch.Draw(
				GetPixel(spriteBatch.GraphicsDevice),
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

		public static void DrawPolygon(this SpriteBatch spriteBatch, IReadOnlyList<Vector2> vertices, Color color, float thickness = 2f)
		{
			if (spriteBatch is null)
			{
				throw new ArgumentNullException(nameof(spriteBatch));
			}

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
				DrawLine(spriteBatch, vertices[prevIndex], vertices[i], color, thickness);
				prevIndex = i;
			}
		}
	}

	public static class Utils
	{
		public static float ToAngle(Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

		public static IEnumerable<Vector2> GetRegularPolygon(float radius, int verticesCount, float offset)
		{
			if (verticesCount < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(verticesCount));
			}

			for (var i = 0; i < verticesCount; i++)
			{
				var angleStep = MathF.Tau / verticesCount * i + offset;

				var x = MathF.Cos(angleStep) * radius;
				var y = MathF.Sin(angleStep) * radius;
				yield return new Vector2(x, y);
			}
		}

		public static bool Contains(IReadOnlyList<Vector2> vertices, Vector2 point)
		{
			var result = false;
			var prevIndex = vertices.Count - 1;
			for (var i = 0; i < vertices.Count; i++)
			{
				var prevPoint = vertices[prevIndex];
				var currentPoint = vertices[i];
				if (currentPoint.Y < point.Y && prevPoint.Y >= point.Y || prevPoint.Y < point.Y && currentPoint.Y >= point.Y)
				{
					if (currentPoint.X + (point.Y - currentPoint.Y) / (prevPoint.Y - currentPoint.Y) * (prevPoint.X - currentPoint.X) < point.X)
					{
						result = !result;
					}
				}
				prevIndex = i;
			}
			return result;
		}
	}
}
