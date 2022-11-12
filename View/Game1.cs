using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.TimeLine;
using StepFlow.View.Controlers;
using StepFlow.ViewModel;

namespace StepFlow.View
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

		private static void RegularPoligon(SpriteBatch spriteBatch, Vector2 center, float radius, int verticesCount, Color color, float offset = 0)
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

			DrawPolygon(spriteBatch, vertices, color);
		}

		private static IEnumerable<Vector2> GetRegularPoligon(float radius, int verticesCount, float offset)
		{
			const float FULL_ROUND = MathF.PI * 2;

			if (verticesCount < 3)
			{
				throw new ArgumentOutOfRangeException(nameof(verticesCount));
			}

			for (var i = 0; i < verticesCount; i++)
			{
				var angleStep = FULL_ROUND / verticesCount * i + offset;

				var x = MathF.Cos(angleStep) * radius;
				var y = MathF.Sin(angleStep) * radius;
				yield return new Vector2(x, y);
			}
		}

		private static float ToAngle(Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

		private static float Size { get; } = 20;
		private static float Width { get; } = Size * 2;
		private static float Height { get; } = MathF.Sqrt(3) * Size;
		private static float CellWidth { get; } = Width / 4;
		private static float CellHeight { get; } = Height / 2;

		private GraphicsDeviceManager Graphics { get; }

		private SpriteBatch? spriteBatch;

		public SpriteBatch SpriteBatch => spriteBatch ?? throw new InvalidOperationException();

		private WorldVm World { get; }
		private MovementPieceView MovementPiece { get; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			World = new(new(10, 10));
			for (var iCol = 0; iCol < World.ColsCount; iCol++)
			{
				for (var iRow = 0; iRow < World.RowsCount; iRow++)
				{
					var cellX = iCol * 3;
					var cellY = iRow * 2;
					if (iCol % 2 == 0)
					{
						cellY++;
					}

					var location = new Vector2(cellX * CellWidth + Size, cellY * CellHeight + Size);

					var view = new HexNodeView(this, World[iCol, iRow])
					{
						Color = Color.Red,
						Size = Size,
						Location = location
					};

					Components.Add(view);
				}
			}

			MovementPiece = new(this, new(new()));
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Pixel = new(GraphicsDevice, 1, 1);
			Pixel.SetData(new[] { Color.White });

			// TODO: use this.Content to load your game content here
		}

		private KeyboardState prevKeyboardState;

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);

		public bool MouseButtonOnPress() => Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed;

		public Point MousePosition() => Mouse.GetState().Position;

		private MouseState prevMouseState;
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
			{
				World.TimeAxis.MoveNext();
			}

			base.Update(gameTime);

			prevKeyboardState = Keyboard.GetState();
			prevMouseState = Mouse.GetState();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			var planings = Commands.Select(x => x.NextNode).ToHashSet();
			for (var iX = 0; iX < TableView.GetLength(0); iX++)
			{
				for (var iY = 0; iY < TableView.GetLength(1); iY++)
				{
					var hexNodeView = TableView[iX, iY];

					if (hexNodeView.Source == MovementPiece.Current)
					{
						hexNodeView.State = NodeState.Current;
					}
					else if (planings.Contains(hexNodeView.Source))
					{
						hexNodeView.State = NodeState.Planned;
					}
					else
					{
						hexNodeView.State = NodeState.Node;
					}

					hexNodeView.Draw(spriteBatch);
				}
			}

			const float CELL_SIZE = 40;
			for (var i = 0; i < MovementPiece.CommandsQueue.Count; i++)
			{
				var position = new Vector2(
					i * CELL_SIZE + 5,
					Graphics.PreferredBackBufferHeight - CELL_SIZE - 5
				);

				DrawPolygon(
					spriteBatch,
					new Vector2[]
					{
						position,
						position + new Vector2(CELL_SIZE, 0),
						position + new Vector2(CELL_SIZE, CELL_SIZE),
						position + new Vector2(0, CELL_SIZE),
					},
					Color.Red,
					1
				);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}