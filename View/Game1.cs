using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TimeLine;

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

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private World World { get; }
		private MovementPiece MovementPiece { get; }

		private static T GetRandomItem<T>(IReadOnlyList<T> collection)
			=> collection[Random.Shared.Next(collection.Count)];

		private HexNodeView[,] TableView { get; }

		private HashSet<MoveCommand> Commands { get; } = new HashSet<MoveCommand>();

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			World = new();
			MovementPiece = new(World, GetRandomItem(World.Grid.Nodes.Keys.ToArray()));

			var xCount = World.Table.GetLength(0);
			var yCount = World.Table.GetLength(1);
			TableView = new HexNodeView[xCount, yCount];
			for (var iX = 0; iX < xCount; iX++)
			{
				for (var iY = 0; iY < yCount; iY++)
				{
					var cellX = iX * 3;
					var cellY = iY * 2;
					if (iX % 2 == 0)
					{
						cellY++;
					}

					var location = new Vector2(cellX * CellWidth + Size, cellY * CellHeight + Size);

					TableView[iX, iY] = new HexNodeView(World.Table[iX, iY])
					{
						Color = Color.Red,
						Size = Size,
						Location = location
					};
				}
			}
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

		private KeyboardState prevKeyboardState;
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
			prevKeyboardState = Keyboard.GetState();

			var mouseState = Mouse.GetState();
			foreach (var view in TableView.OfType<HexNodeView>())
			{
				view.IsSelected = view.Contains(mouseState.Position);
				if (view.IsSelected && mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
				{
					MovementPiece.Enqueue(new MoveCommand(MovementPiece, view.Source, Commands));
				}
			}

			prevMouseState = mouseState;

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

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

					hexNodeView.Draw(_spriteBatch);
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

		public class HexNodeView
		{
			public HexNodeView(HexNode source)
			{
				Source = source ?? throw new ArgumentNullException(nameof(source));
			}

			public HexNode Source { get; }

			private Vector2 location = Vector2.Zero;
			public Vector2 Location
			{
				get => location;
				set
				{
					if (Location != value)
					{
						location = value;
						ClearCache();
					}
				}
			}

			private float size = 0;
			public float Size
			{
				get => size;
				set
				{
					if (Size != value)
					{
						size = value;
						ClearCache();
					}
				}
			}

			public Color Color { get; set; } = Color.Black;

			public NodeState State { get; set; } = NodeState.Node;

			public bool IsSelected { get; set; } = false;

			private void ClearCache()
			{
				vertices = null;
				innerVertices = null;
			}

			private Vector2[]? vertices = null;
			private Vector2[] Vertices => vertices ??= GetRegularPoligon(Size, 6, 0).Select(x => x + Location).ToArray();

			private Vector2[]? innerVertices = null;
			private Vector2[] InnerVertices => innerVertices ??= GetRegularPoligon(Size * 0.8f, 6, 0).Select(x => x + Location).ToArray();

			public void Draw(SpriteBatch spriteBatch)
			{
				DrawPolygon(spriteBatch, Vertices, IsSelected ? Color.Blue : Color);

				Color? stateColor = null;
				switch (State)
				{
					case NodeState.Current:
						stateColor = Color.Green;
						break;
					case NodeState.Planned:
						stateColor = Color.Yellow;
						break;
				}

				if (stateColor is { } color)
				{
					DrawPolygon(spriteBatch, InnerVertices, color);
				}
			}

			public bool Contains(Point point)
			{
				var result = false;
				var prevIndex = Vertices.Length - 1;
				for (int i = 0; i < Vertices.Length; i++)
				{
					var prevPoint = Vertices[prevIndex];
					var currentPoint = Vertices[i];
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

		public enum NodeState
		{
			Node,
			Current,
			Planned,
		}
	}

	public class MoveCommand : CommandBase
	{
		public MoveCommand(MovementPiece owner, HexNode nextNode, ICollection<MoveCommand> container)
		{
			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
			NextNode = nextNode ?? throw new ArgumentNullException(nameof(nextNode));
			Container = container ?? throw new ArgumentNullException(nameof(container));

			Container.Add(this);
		}

		public MovementPiece Owner { get; }

		public HexNode NextNode { get; }

		public ICollection<MoveCommand> Container { get; }

		public override void Execute()
		{
			Owner.Current = NextNode;
		}

		public override void Dispose()
		{
			Container.Remove(this);
		}
	}
}