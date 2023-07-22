using System.Net.NetworkInformation;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View
{
	public class GameHandler : ILayoutCanvas
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Font = game.Content.Load<SpriteFont>("DefaultFont");

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice);

			game.Services.AddService<IMouseService>(MouseService);
			game.Services.AddService<IKeyboardService>(KeyboardService);
			game.Services.AddService<IDrawer>(Drawer);

			Place = bounds;

			Base = new Primitive(game.Services);

			Base.Childs.Add(new Polygon(game.Services)
			{
				Color = Color.Yellow,
				Thickness = 5,
				Vertices = new VerticesCollection()
				{
					new(0, 0),
					new(100, 0),
					new(100, 100),
					new(0, 100),
				},
			});

			Base.Childs.Add(new Polygon(game.Services)
			{
				Color = Color.Red,
				Thickness = 6,
				Vertices = new VerticesCollection()
				{
					new(200, 200),
					new(230, 200),
					new(230, 230),
				},
			});

			var b = new LayoutControl(game.Services)
			{
				Layout = new Layout()
				{
					Canvas = this,
					Margin = new(30),
				},
			};
			Base.Childs.Add(b);

			Base.Childs.Add(new Text(game.Services)
			{
				Content = "My text!",
				Color = Color.Pink,
				HorizontalAlign = HorizontalAlign.Right,
				VerticalAlign = VerticalAlign.Center,
				Font = Font,
				Layout = new Layout()
				{
					Canvas = b,
					Size = new(150, 100),
					Margin = new(15)
					{
						Left = new Unit(0, UnitKind.None),
					},
				},
			});

			Base.Childs.Add(new Hex(game.Services)
			{
				Position = new(200, 100),
				Orientation = HexOrientation.Pointy,
				Size = 30,
			});
		}

		private Primitive Base { get; }

		private SpriteFont Font { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

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