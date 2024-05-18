using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Common.Exceptions;
using StepFlow.Markup.Services;

namespace StepFlow.View.Services
{
	public sealed class Drawer : IDrawer
	{
		private const string DEFAULT_FONT_KEY = "DefaultFont";
		private const string DEFAULT_TILES_KEY = "DefaultTiles";
		private const string PLACE_TILES_KEY = "PlaceTiles";

		public Drawer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ContentManager content)
		{
			SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
			GraphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
			Content = content ?? throw new ArgumentNullException(nameof(content));

			DefaultFont = content.Load<SpriteFont>(DEFAULT_FONT_KEY);

			DefaultTiles = content.Load<Texture2D>(DEFAULT_TILES_KEY);
			PlaceTiles = content.Load<Texture2D>(PLACE_TILES_KEY);

			Sprites.Add(Markup.Services.Texture.Circle, new(DefaultTiles, new(18, 673, 32, 32)));

			Sprites.Add(Markup.Services.Texture.Character, new(DefaultTiles, new(414, 53, 44, 59)));
			Sprites.Add(Markup.Services.Texture.Projectile, new(DefaultTiles, new(243, 256, 20, 19)));
			Sprites.Add(Markup.Services.Texture.ProjectileFire, new(DefaultTiles, new(211, 256, 20, 19)));
			Sprites.Add(Markup.Services.Texture.ProjectilePoison, new(DefaultTiles, new(51, 256, 20, 19)));
			Sprites.Add(Markup.Services.Texture.ProjectileAll, new(DefaultTiles, new(147, 256, 20, 19)));
			Sprites.Add(Markup.Services.Texture.Enemy, new(DefaultTiles, new(662, 22, 56, 81)));

			Sprites.Add(Markup.Services.Texture.ObstructionNone, new(DefaultTiles, new(768, 704, 32, 32)));
			Sprites.Add(Markup.Services.Texture.ObstructionDarkWall, new(DefaultTiles, new(518, 690, 32, 32)));
			Sprites.Add(Markup.Services.Texture.ObstructionBricks, new(PlaceTiles, new(384, 320, 32, 32)));
			Sprites.Add(Markup.Services.Texture.ObstructionBricksDamaged, new(PlaceTiles, new(480, 384, 32, 32)));
			Sprites.Add(Markup.Services.Texture.ObstructionBoards, new(PlaceTiles, new(64, 256, 32, 32)));

			Sprites.Add(Markup.Services.Texture.ItemPoison, new(DefaultTiles, new(523, 827, 22, 21)));
			Sprites.Add(Markup.Services.Texture.ItemFire, new(DefaultTiles, new(552, 833, 29, 16)));
			Sprites.Add(Markup.Services.Texture.ItemUnknown, new(DefaultTiles, new(517, 561, 13, 13)));
			Sprites.Add(Markup.Services.Texture.Floor, new(PlaceTiles, new(640, 0, 32, 32)));
			Sprites.Add(Markup.Services.Texture.ItemSpeed, new(DefaultTiles, new(20, 158, 18, 22)));
			Sprites.Add(Markup.Services.Texture.ItemAttackSpeed, new(DefaultTiles, new(51, 317, 24, 26)));
			Sprites.Add(Markup.Services.Texture.ItemAddStrength, new(DefaultTiles, new(339, 573, 10, 9)));
			Sprites.Add(Markup.Services.Texture.PoisonPlace, new(PlaceTiles, new(352, 736, 32, 32)));

			Pixel = new(graphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });
		}

		private SpriteFont DefaultFont { get; }

		private Texture2D DefaultTiles { get; }

		private Texture2D PlaceTiles { get; }

		private Dictionary<Markup.Services.Texture, Sprite> Sprites { get; } = new();

		private Texture2D Pixel { get; }

		public SpriteBatch SpriteBatch { get; }

		public GraphicsDevice GraphicsDevice { get; }

		public ContentManager Content { get; }

		private static Color ToMonoColor(System.Drawing.Color color) => new(color.R, color.G, color.B, color.A);

		public void Line(System.Drawing.PointF start, System.Drawing.PointF end, System.Drawing.Color color, float thickness = 2f)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(thickness);

			Vector2 delta = ((System.Numerics.Vector2)end) - ((System.Numerics.Vector2)start);
			SpriteBatch.Draw(
				Pixel,
				new(start.X, start.Y),
				null,
				ToMonoColor(color),
				Utils.ToAngle(delta),
				new Vector2(0, 0.5f),
				new Vector2(delta.Length(), thickness),
				SpriteEffects.None,
				0f
			);
		}

		public void Polygon(IReadOnlyList<System.Drawing.PointF> vertices, System.Drawing.Color color, float thickness = 2f)
		{
			ArgumentNullException.ThrowIfNull(vertices);
			ArgumentOutOfRangeException.ThrowIfNegative(thickness);

			var prevIndex = vertices.Count - 1;
			for (var i = 0; i < vertices.Count; i++)
			{
				Line(vertices[prevIndex], vertices[i], color, thickness);
				prevIndex = i;
			}
		}

		public void DrawString(string text, System.Drawing.PointF position, System.Drawing.Color color)
			=> SpriteBatch.DrawString(
				DefaultFont,
				text,
				new Vector2(position.X, position.Y),
				ToMonoColor(color)
			);

		public void DrawString(string text, System.Drawing.RectangleF bounds, HorizontalAlign horizontalAlign, VerticalAlign verticalAlign, System.Drawing.Color color)
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

			DrawString(text, new(x, y), color);
		}

		public Vector2 MeasureString(string text) => DefaultFont.MeasureString(text);

		public void Draw(Markup.Services.Texture texture, System.Drawing.Rectangle rectangle, System.Drawing.Color? color)
		{
			var sprite = Sprites[texture];
			SpriteBatch.Draw(
				sprite.Texture,
				new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height),
				sprite.SourceRectangle,
				ToMonoColor(color ?? System.Drawing.Color.White)
			);
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
