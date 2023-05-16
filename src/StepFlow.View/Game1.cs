using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;
using StepFlow.ViewModel.Layout;

namespace StepFlow.View
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager Graphics { get; }

		private SpriteBatch? spriteBatch;

		public SpriteBatch SpriteBatch => spriteBatch ?? throw new InvalidOperationException();

		private RootVm Root { get; set; }

		private SpriteFont Font { get; set; }

		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; set; }

		private Sketch.Primitive Base { get; set; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Font = Content.Load<SpriteFont>("DefaultFont");

			Drawer = new Drawer(spriteBatch, GraphicsDevice);

			Services.AddService<IMouseService>(MouseService);
			Services.AddService<IKeyboardService>(KeyboardService);
			Services.AddService<IDrawer>(Drawer);

			Root = new RootVm(new ServiceCollection().BuildServiceProvider(), 3, 3);
			Root.Root.OwnerBounds = new System.Drawing.RectangleF(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
			Root.Root.Margin = new Layout.Margin(1);

			Base = new Sketch.Primitive(this);
			Base.Childs.Add(new GridControl(this, Root.Root));

			var hexGrid = new HexGrid(this, Root.Context, Root.ActionPlot)
			{
				Size = 20,
			};
			Base.Childs.Add(hexGrid);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			Update(Base, gameTime);
			base.Update(gameTime);

			KeyboardService.Update();
			MouseService.Update();
		}

		private void Update(Primitive primitive, GameTime gameTime)
		{
			if (primitive.Enable)
			{
				primitive.Update(gameTime);

				foreach (var child in primitive.Childs)
				{
					Update(child, gameTime);
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);
			base.Draw(gameTime);

			SpriteBatch.End();
		}

		private void Draw(Primitive primitive, GameTime gameTime)
		{
			if (primitive.Visible)
			{
				primitive.Draw(gameTime);

				foreach (var child in primitive.Childs)
				{
					Draw(child, gameTime);
				}
			}
		}
	}
}