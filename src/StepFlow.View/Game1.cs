using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.View.Services;

namespace StepFlow.View
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager Graphics { get; }

		private GameRunner? Runner { get; set; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			var spriteBatch = new SpriteBatch(GraphicsDevice);
			var keyboard = new ControlService();
			var drawer = new Drawer(spriteBatch, GraphicsDevice, Content);
			Runner = new(
				spriteBatch,
				keyboard,
				new(
					new System.Drawing.RectangleF(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight),
					drawer,
					keyboard
				)
			);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			Runner?.Update();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			Runner?.Draw();
		}

		private sealed class GameRunner
		{
			public GameRunner(SpriteBatch spriteBatch, ControlService controlService, Markup.GameHandler handler)
			{
				SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
				ControlService = controlService ?? throw new ArgumentNullException(nameof(controlService));
				Handler = handler ?? throw new ArgumentNullException(nameof(handler));
			}

			public SpriteBatch SpriteBatch { get; }

			public ControlService ControlService { get; }

			public Markup.GameHandler Handler { get; }

			public void Update()
			{
				Handler.Update();

				ControlService.Update();
			}

			public void Draw()
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

				Handler.Draw();

				SpriteBatch.End();
			}
		}
	}
}