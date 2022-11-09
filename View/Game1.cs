using System;
using System.Collections.Generic;
using System.Linq;
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

		private static float ToAngle(Vector2 vector) => MathF.Atan2(vector.Y, vector.X);

		private static float Size { get; } = 20;
		private static float Width { get; } = Size * 2;
		private static float Height { get; } = MathF.Sqrt(3) * Size;
		private static float CellWidth { get; } = Width / 4;
		private static float CellHeight { get; } = Height / 2;

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private World World { get; }
		private MovementPiece MovementPiece { get; }

		private static T GetRandomItem<T>(IReadOnlyList<T> collection)
			=> collection[Random.Shared.Next(collection.Count)];

		private void NextMove()
		{
			var applicantsNodes = World.Grid.Edges.Keys
				.Where(x => x.Begin == MovementPiece.Current)
				.Select(x => x.End)
				.ToArray();

			NextNode = GetRandomItem(applicantsNodes);
			MovementPiece.MoveTo(NextNode);
		}

		private HexNode NextNode { get; set; }

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			World = new();
			MovementPiece = new(World, GetRandomItem(World.Grid.Nodes.Keys.ToArray()));

			NextMove();
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

		private KeyboardState prevState;
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevState.IsKeyDown(Keys.Space))
			{
				World.TimeAxis.MoveNext();

				NextMove();
			}
			prevState = Keyboard.GetState();
			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			var locations = new Vector2[World.Table.GetLength(0), World.Table.GetLength(1)];
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

					var location = new Vector2(cellX * CellWidth + Size, cellY * CellHeight + Size);
					locations[iX, iY] = location;
					RegularPoligon(
						_spriteBatch,
						location,
						Size,
						6,
						Color.Red
					);

					if (MovementPiece.Current == World.Table[iX, iY])
					{
						RegularPoligon(
							_spriteBatch,
							location,
							Size * 0.8f,
							6,
							Color.Green
						);
					}
					if (NextNode == World.Table[iX, iY])
					{
						RegularPoligon(
							_spriteBatch,
							location,
							Size * 0.8f,
							6,
							Color.Yellow
						);
					}
				}
			}

			foreach (var begin in World.Grid.Nodes.Keys)
			{
				foreach (var end in World.Grid.Nodes[begin].AsBegin)
				{
					DrawLine(
						_spriteBatch,
						locations[begin.Col, begin.Row],
						locations[end.End.Col, end.End.Row],
						Color.Blue,
						1
					);
				}
			}

			const float CELL_SIZE = 40;
			for (var i = 0; i < MovementPiece.CommandsQueue.Count; i++)
			{
				var position = new Vector2(
					i * CELL_SIZE + 5,
					_graphics.PreferredBackBufferHeight - CELL_SIZE - 5
				);

				DrawPolygon(
					_spriteBatch,
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

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}