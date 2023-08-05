using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StepFlow.Core;
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

			var bordered0 = CreateBordered(10);
			bordered0.Offset(new(20, 20));
			var bordered1 = CreateBordered(17);
			bordered1.Offset(new(30, 20));

			const int SIZE = 10;
			const float OFFSET = 0.25f;
			for (var iX = 0; iX < 50; iX++)
			{
				for (var iY = 0; iY < 50; iY++)
				{
					var position = new System.Drawing.Point(iX, iY);
					Color color;
					if (bordered0.Contains(position))
					{
						color = bordered1.Contains(position) ? Color.Yellow : Color.Green;
					}
					else
					{
						color = bordered1.Contains(position) ? Color.Blue : Color.Red;
					}

					Base.Childs.Add(new Polygon(game.Services)
					{
						Color = color,
						Thickness = 1,
						Vertices = new BoundsVertices()
						{
							Bounds = new System.Drawing.RectangleF(
								iX * SIZE + OFFSET,
								iY * SIZE + OFFSET,
								SIZE - OFFSET * 2,
								SIZE - OFFSET * 2
							),
						},
					});
				}
			}
		}

		private Bordered CreateBordered(int diameter)
		{
			var bordered = new StepFlow.Core.Bordered();
			foreach (var point in Create(diameter))
			{
				bordered.AddCell(new(point, new System.Drawing.Size(1, 1)));
			}
			return bordered;
		}

		private IEnumerable<System.Drawing.Point> Create(int diameter)
		{
			if (diameter == 0)
			{
				yield return System.Drawing.Point.Empty;
			}
			else
			{
				var begin = diameter / 2;
				var radius = diameter / 2f;
				var radiusD = radius * radius;
				for (var iX = -begin; iX < diameter; iX++)
				{
					for (var iY = -begin; iY < diameter; iY++)
					{
						if ((iY * iY) + (iX * iX) <= radiusD)
						{
							yield return new(iX, iY);
						}
					}
				}
			}
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