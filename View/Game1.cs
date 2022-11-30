using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.View.Controls;
using StepFlow.View.Layout;
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

		private GridPlot Root { get; }

		private SubPlotRect MainPlace { get; }

		private GridPlot Queue { get; }

		public Game1()
		{
			Graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			World = new WorldVm(new Core.World(10, 10));
			World.PropertyChanging += WorldPropertyChanging;
			World.PropertyChanged += WorldPropertyChanged;

			Root = new GridPlot()
			{
				OwnerBounds = new System.Drawing.RectangleF(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight),
				Margin = new Margin(0),
			};
			MainPlace = new SubPlotRect()
			{
				Margin = new Margin(0),
			};
			Queue = new GridPlot()
			{
				Margin = new Margin(0),
			};
			Root.Add(MainPlace, new CellPosition(0, 0));
			Root.Add(Queue, new CellPosition(0, 1));
		}

		private void WorldPropertyChanging(object? sender, PropertyChangingEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(WorldVm.Current):
					if (World.Current is not null)
					{
						World.Current.CommandQueue.CollectionChanged -= WorldCurrentCommandQueueCollectionChanged;
					}
					break;
			}
		}

		private void WorldPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(WorldVm.Current):
					if (World.Current is not null)
					{
						World.Current.CommandQueue.CollectionChanged += WorldCurrentCommandQueueCollectionChanged;
					}
					break;
			}
		}

		private void WorldCurrentCommandQueueCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			
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