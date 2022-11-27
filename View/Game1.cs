using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.View.Controlers;
using StepFlow.ViewModel;

namespace StepFlow.View
{
	public class Game1 : Game, IPlace
	{
		private static float Size { get; } = 20;
		private static float Width { get; } = Size * 2;
		private static float Height { get; } = MathF.Sqrt(3) * Size;
		private static float CellWidth { get; } = Width / 4;
		private static float CellHeight { get; } = Height / 2;

		private GraphicsDeviceManager Graphics { get; }

		private SpriteBatch? spriteBatch;

		public SpriteBatch SpriteBatch => spriteBatch ?? throw new InvalidOperationException();

		private WorldVm World { get; }
		private IPieceVm CurrentPiece { get; }
		private List<RectView> Commands { get; } = new List<RectView>();

		public System.Drawing.RectangleF Place { get; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Place = new System.Drawing.RectangleF(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);

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

			CurrentPiece = World[0, 0].CreateSimple();
			World.Current = CurrentPiece;

			Components.Add(new AxisView(this, World.TimeAxis));

			var grid = new GridTest(this)
			{
				PlaceOwner = this,
				Columns =
				{
					new CellSize(0.5f, UnitMeasure.Ptc),
					new CellSize(0.5f, UnitMeasure.Ptc),
				},
				Rows =
				{
					new CellSize(0.7f, UnitMeasure.Ptc),
					new CellSize(0.3f, UnitMeasure.Ptc),
				},
				Margin = new Margin(5, 5, null, 5),
				Size = new(0, 30),
			};
			Components.Add(grid);

			var test = new Test(this)
			{
				Margin = new Margin(2),
			};
			grid.Add(test, new(0, 0, 2, 1));
			Components.Add(test);
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		private KeyboardState prevKeyboardState;

		public bool IsKeyOnPress(Keys key) => Keyboard.GetState().IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);

		public bool MouseButtonOnPress() => Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed;

		public Point MousePosition() => Mouse.GetState().Position;

		private MouseState prevMouseState;

		public event PropertyChangedEventHandler? PropertyChanged;

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			base.Update(gameTime);

			prevKeyboardState = Keyboard.GetState();
			prevMouseState = Mouse.GetState();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			base.Draw(gameTime);

			SpriteBatch.End();
		}
	}

	public class GridTest : GridLayout
	{
		public GridTest(Game1 game) : base(game)
		{
		}

		public override void Draw(GameTime gameTime)
		{
			var bounds = Bound;
			Game.SpriteBatch.DrawPolygon(
				new Vector2[]
				{
					new(bounds.Left, bounds.Top),
					new(bounds.Right, bounds.Top),
					new(bounds.Right, bounds.Bottom),
					new(bounds.Left, bounds.Bottom),
				},
				Color.Green
			);

			base.Draw(gameTime);
		}
	}

	public class Test : ViewLayout
	{
		public Test(Game1 game) : base(game)
		{
		}

		public override void Draw(GameTime gameTime)
		{
			var bounds = Bound;
			Game.SpriteBatch.DrawPolygon(
				new Vector2[]
				{
					new(bounds.Left, bounds.Top),
					new(bounds.Right, bounds.Top),
					new(bounds.Right, bounds.Bottom),
					new(bounds.Left, bounds.Bottom),
				},
				Color.Red
			);

			base.Draw(gameTime);
		}
	}
}