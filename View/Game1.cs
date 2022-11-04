using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace View
{
	public class Game1 : Game
	{
		private static Texture2D Pixel { get; set; }

		private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 2f)
		{
			Vector2 delta = end - start;
			spriteBatch.Draw(Pixel, start, null, color, ToAngle(delta), new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
		}

		private static void DrawPolygon(SpriteBatch spriteBatch, IReadOnlyList<Vector2> vertices, Color color, float thickness = 2f)
		{
			var prevIndex = vertices.Count - 1;
			for (var i = 0; i < vertices.Count; i++)
			{
				DrawLine(spriteBatch, vertices[prevIndex], vertices[i], color, thickness);
				prevIndex = i;
			}
		}

		private static void RegularPoligon(SpriteBatch spriteBatch, Vector2 center, float radius, int verticesCount, float offset = 0)
		{
			const float FULL_ROUND = MathF.PI * 2;

			if (verticesCount < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(verticesCount));
			}

			var vertices = new Vector2[verticesCount];
			for (var i = 0; i < verticesCount; i++)
			{
				var angleStep = FULL_ROUND / verticesCount * i + offset;

				var x = MathF.Cos(angleStep) * radius;
				var y = MathF.Sin(angleStep) * radius;
				vertices[i] = new Vector2(x, y) + center;
			}

			DrawPolygon(spriteBatch, vertices, Color.Red);
		}

		private static float ToAngle(Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

		private static float Size { get; } = 15;
		private static float Width { get; } = Size * 2;
		private static float Height { get; } = MathF.Sqrt(3) * Size;
		private static float CellWidth { get; } = Width / 4;
		private static float CellHeight { get; } = Height / 2;

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private World World { get; }

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			World = new();
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			Pixel = new(GraphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			for (var iX = 0; iX < World.Table.GetLength(0); iX++)
			{
				for (var iY = 0; iY < World.Table.GetLength(1); iY++)
				{
					var cellX = iX * 3;
					var cellY = iY * 2;
					if (iX % 2 == 0)
					{
						cellY++;
					}

					RegularPoligon(_spriteBatch, new(cellX * CellWidth + Size, cellY * CellHeight + Size), Size, 6);
				}
			}

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}