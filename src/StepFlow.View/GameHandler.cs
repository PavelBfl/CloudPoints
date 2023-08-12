using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Xml.Schema;
using Microsoft.Extensions.DependencyInjection;
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

			TestVm = new TestVm(Base, 100, 100, 5, 0.25f, game.Services);
		}

		private TestVm TestVm { get; }

		private Primitive Base { get; }

		private SpriteFont Font { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

		public void Update(GameTime gameTime)
		{
			TestVm.Update();
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

	internal sealed class TestVm
	{
		public TestVm(Primitive owner, int width, int height, float size, float offset, IServiceProvider serviceProvider)
		{
			if (owner is null)
			{
				throw new ArgumentNullException(nameof(owner));
			}

			if (serviceProvider is null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			KeyboardService = serviceProvider.GetRequiredService<IKeyboardService>();

			Cells = new Polygon[width, height];
			for (var iX = 0; iX < width; iX++)
			{
				for (var iY = 0; iY < height; iY++)
				{
					Cells[iX, iY] = new Polygon(serviceProvider)
					{
						Color = Microsoft.Xna.Framework.Color.Red,
						Thickness = 1,
						Vertices = new BoundsVertices()
						{
							Bounds = new System.Drawing.RectangleF(
								iX * size + offset,
								iY * size + offset,
								size - offset * 2,
								size - offset * 2
							),
						},
					};
					owner.Childs.Add(Cells[iX, iY]);
				}
			}

			var piecesCount = 2;
			for (var i = 0; i < piecesCount; i++)
			{
				AddPieces(10);
				Current.Offset(new(Random.Shared.Next(100), Random.Shared.Next(100)));
			}
		}

		private IKeyboardService KeyboardService { get; }

		private Polygon[,] Cells { get; }

		private Bordered? Current { get; set; }

		private List<Bordered> Pieces { get; } = new List<Bordered>();

		public void AddPieces(int diameter)
		{
			Current = CreateBordered(diameter);
			Pieces.Add(Current);
		}

		private static Bordered CreateBordered(int diameter)
		{
			var bordered = new StepFlow.Core.Bordered();
			foreach (var point in Create(diameter))
			{
				bordered.AddCell(new(point, new System.Drawing.Size(1, 1)));
			}
			return bordered;
		}

		private static IEnumerable<System.Drawing.Point> Create(int diameter)
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

		public void Update()
		{
			var sw = Stopwatch.StartNew();
			for (var iX = 0; iX < Cells.GetLength(0); iX++)
			{
				for (var iY = 0; iY < Cells.GetLength(1); iY++)
				{
					var count = Pieces.Where(x => x.Contains(new(iX, iY))).Count();
					Cells[iX, iY].Color = count switch
					{
						0 => Microsoft.Xna.Framework.Color.Red,
						1 => Microsoft.Xna.Framework.Color.Green,
						_ => Microsoft.Xna.Framework.Color.Lerp(
							Microsoft.Xna.Framework.Color.Yellow,
							Microsoft.Xna.Framework.Color.Blue,
							Math.Min(count - 2, 5) / 5f
						),
					};
				}
			}
			System.Diagnostics.Trace.WriteLine(sw.Elapsed);

			if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Space))
			{
				AddPieces(10);
			}

			if (Current is not null)
			{
				if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Up))
				{
					Current.Offset(new System.Drawing.Point(0, -1));
				}
				else if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Down))
				{
					Current.Offset(new System.Drawing.Point(0, 1));
				}
				else if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Left))
				{
					Current.Offset(new System.Drawing.Point(-1, 0));
				}
				else if (KeyboardService.IsKeyOnPress(Microsoft.Xna.Framework.Input.Keys.Right))
				{
					Current.Offset(new System.Drawing.Point(1, 0));
				}
			}
		}
	}
}