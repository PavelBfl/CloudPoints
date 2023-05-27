using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;
using StepFlow.ViewModel;
using StepFlow.ViewModel.Layout;

namespace StepFlow.View
{
	public class GameHandler
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Font = game.Content.Load<SpriteFont>("DefaultFont");

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice);

			game.Services.AddService<IMouseService>(MouseService);
			game.Services.AddService<IKeyboardService>(KeyboardService);
			game.Services.AddService<IDrawer>(Drawer);

			var wrapperProvider = new WrapperProvider();
			Root = new RootVm(wrapperProvider.GetOrCreate<ContextVm>(new GamePlay.Context()));

			Root.Context.World.Place.Add(wrapperProvider.GetOrCreate<NodeVm>(new GamePlay.Node(new(0, 0))));
			Root.Context.World.Place.Add(wrapperProvider.GetOrCreate<NodeVm>(new GamePlay.Node(new(1, 1))));
			Root.Context.World.Place.Add(wrapperProvider.GetOrCreate<NodeVm>(new GamePlay.Node(new(1, 2))));

			Root.Root.OwnerBounds = bounds;
			Root.Root.Margin = new Layout.Margin(1);

			Base = new Sketch.Primitive(game);
			Base.Childs.Add(new GridControl(game, Root.Root));

			var hexGrid = new HexGrid(game, Root.ActionPlot)
			{
				Source = Root.Context.World,
				Size = 20,
			};
			Base.Childs.Add(hexGrid);
		}

		private RootVm Root { get; }

		private Sketch.Primitive Base { get; }

		private SpriteFont Font { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public void Update(GameTime gameTime)
		{
			Update(Base, gameTime);

			KeyboardService.Update();
			MouseService.Update();
		}

		private static void Update(Primitive primitive, GameTime gameTime)
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

		public void Draw(GameTime gameTime)
		{
			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);

			SpriteBatch.End();
		}

		private static void Draw(Primitive primitive, GameTime gameTime)
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