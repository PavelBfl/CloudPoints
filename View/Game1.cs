using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.View.Controls;
using StepFlow.ViewModel.Layout;

namespace StepFlow.View
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager Graphics { get; }

		private SpriteBatch? spriteBatch;

		public SpriteBatch SpriteBatch => spriteBatch ?? throw new InvalidOperationException();

		private RootVm Root { get; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			Root = new RootVm(10, 10, HexOrientation.Flat, false);
			Root.Root.OwnerBounds = new System.Drawing.RectangleF(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
			Root.Root.Margin = new Layout.Margin(1);
			Root.Root.Refresh();

			Components.Add(new GridControl(this, Root.Root));

			Root.World.Current = Root.World[0, 0].CreateSimple();

			Root.World.Current.MoveTo(Root.World[0, 1]);
			Root.World.Current.MoveTo(Root.World[0, 2]);
			Root.World.Current.MoveTo(Root.World[0, 3]);
			Root.World.Current.MoveTo(Root.World[0, 4]);

			var hexGrid = new HexGrid(this, Root.World, Root.ActionPlot)
			{
				Size = 20,
			};
			Components.Add(hexGrid);
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