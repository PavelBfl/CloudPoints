using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Common.Exceptions;
using StepFlow.View.Controls;

namespace StepFlow.View.Services
{
	public sealed class Drawer : IDrawer
	{
		private const string DEFAULT_FONT_KEY = "DefaultFont";
		private const string DEFAULT_TILES_KEY = "DefaultTiles";

		public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content)
		{
			SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
			Content = content ?? throw new ArgumentNullException(nameof(content));

			DefaultFont = content.Load<SpriteFont>(DEFAULT_FONT_KEY);

			DefaultTiles = content.Load<Texture2D>(DEFAULT_TILES_KEY);
			Sprites.Add("Character", new(DefaultTiles, new(414, 53, 44, 59)));
			Sprites.Add("Projectile", new(DefaultTiles, new(243, 256, 20, 19)));
			Sprites.Add("ProjectileFire", new(DefaultTiles, new(211, 256, 20, 19)));
			Sprites.Add("ProjectilePoison", new(DefaultTiles, new(51, 256, 20, 19)));
			Sprites.Add("ProjectileAll", new(DefaultTiles, new(147, 256, 20, 19)));
			Sprites.Add("Enemy", new(DefaultTiles, new(662, 22, 56, 81)));

			Pixel = new(graphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });
		}

		private SpriteFont DefaultFont { get; }

		private Texture2D DefaultTiles { get; }

		private Dictionary<string, Sprite> Sprites { get; } = new();

		private Texture2D Pixel { get; }

		public SpriteBatch SpriteBatch { get; }

		public GraphicsDevice GraphicsDevice { get; }

		public ContentManager Content { get; }

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

		public void DrawString(string text, Vector2 position, Color color)
			=> SpriteBatch.DrawString(DefaultFont, text, position, color);

		public void DrawString(string text, System.Drawing.RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, Color color)
		{
			var contentSize = MeasureString(text);
			var x = horizontalAlign switch
			{
				HorizontalAlign.Left => bounds.Left,
				HorizontalAlign.Center => bounds.Left + (bounds.Width - contentSize.X) / 2,
				HorizontalAlign.Right => bounds.Right - contentSize.X,
				_ => throw EnumNotSupportedException.Create(horizontalAlign),
			};

			var y = verticalAlign switch
			{
				VerticalAlign.Top => bounds.Top,
				VerticalAlign.Center => bounds.Top + (bounds.Height - contentSize.Y) / 2,
				VerticalAlign.Bottom => bounds.Bottom - contentSize.Y,
				_ => throw EnumNotSupportedException.Create(verticalAlign),
			};

			DrawString(text, new Vector2(x, y), color);
		}

		public Vector2 MeasureString(string text) => DefaultFont.MeasureString(text);

		public void Draw(string texture, Rectangle rectangle, Color? color)
		{
			if (string.IsNullOrWhiteSpace(texture))
			{
				throw new ArgumentException($"\"{nameof(texture)}\" не может быть пустым или содержать только пробел.", nameof(texture));
			}

			var sprite = Sprites[texture];
			SpriteBatch.Draw(sprite.Texture, rectangle, sprite.SourceRectangle, color ?? Color.White);
		}

		private readonly struct Sprite
		{
			public Sprite(Texture2D texture, Rectangle sourceRectangle)
			{
				Texture = texture ?? throw new ArgumentNullException(nameof(texture));
				SourceRectangle = sourceRectangle;
			}

			public Texture2D Texture { get; }

			public Rectangle SourceRectangle { get; }
		}
	}
}
