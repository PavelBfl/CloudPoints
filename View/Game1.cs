using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.View.Controlers;
using StepFlow.ViewModel;

namespace StepFlow.View
{
	public class Game1 : Game
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

			CurrentPiece = World[0, 0].CreateSimple();
			World.Current = CurrentPiece;

			Components.Add(new AxisView(this, World.TimeAxis));
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
}