﻿using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Master;
using StepFlow.Master.Proxies;
using StepFlow.View.Controls;
using StepFlow.View.Services;
using StepFlow.View.Sketch;

namespace StepFlow.View
{
	public class GameHandler
	{
		public GameHandler(Game1 game, System.Drawing.RectangleF bounds)
		{
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);

			Drawer = new Drawer(SpriteBatch, game.GraphicsDevice, game.Content);

			game.Services.AddService<IMouseService>(MouseService);
			game.Services.AddService<IKeyboardService>(KeyboardService);
			game.Services.AddService<IDrawer>(Drawer);

			Place = bounds;

			Base = new Primitive(game.Services);

			Meter.CreateObservableGauge("Time", () => PlayMaster.Time);
			Init();
		}

		private Meter Meter { get; } = new Meter("Game.Gameplay");

		private PlayMaster PlayMaster { get; } = new PlayMaster();

		private Primitive Base { get; }

		private SpriteBatch SpriteBatch { get; }


		private MouseService MouseService { get; } = new MouseService();

		private KeyboardService KeyboardService { get; } = new KeyboardService();

		private Drawer Drawer { get; }

		public System.Drawing.RectangleF Place { get; }

		public void Init()
		{
			CreatePlayerCharacter(new(100, 100, 20, 20), 100);

			CreateRoom(new(5, 5), new(40, 20), 10);
		}

		private void CreatePlayerCharacter(System.Drawing.Rectangle bounds, float strength)
		{
			PlayMaster.Execute(@$"
				playground.CreatePlayerCharacter(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{strength}
				);
			");
		}

		private void CreateRoom(System.Drawing.Point location, System.Drawing.Size size, int width)
		{
			for (var iX = 0; iX < size.Width; iX++)
			{
				CreateBounds(new(iX, 0));
			}

			for (var iY = 0; iY < size.Height; iY++)
			{
				CreateBounds(new(size.Width, iY));
			}

			for (var iX = size.Width; iX > 0; iX--)
			{
				CreateBounds(new(iX, size.Height));
			}

			for (var iY = size.Height; iY > 0; iY--)
			{
				CreateBounds(new(0, iY));
			}

			void CreateBounds(System.Drawing.Point index)
			{
				var tileLocation = new System.Drawing.Point(index.X * width + location.X, index.Y * width + location.Y);
				var bounds = new System.Drawing.Rectangle(tileLocation.X, tileLocation.Y, width, width);
				CreateObstruction(bounds, null);
			}
		}

		private void CreateObstruction(System.Drawing.Rectangle bounds, float? strength)
		{
			PlayMaster.Execute(@$"
				playground.CreateObstruction(
					playground.CreateRectangle({bounds.X}, {bounds.Y}, {bounds.Width}, {bounds.Height}),
					{strength?.ToString() ?? "null"}
				);
			");
		}

		private void PlayerCharacterSetCourse(Course course)
		{
			PlayMaster.Execute($@"
				playground.PlayerCharacter.SetCourse({(int)course});
			");
		}

		public void Update(GameTime gameTime)
		{
			if (KeyboardService.IsKeyDown(Keys.Left))
			{
				PlayerCharacterSetCourse(Course.Left);
			}
			else if (KeyboardService.IsKeyDown(Keys.Up))
			{
				PlayerCharacterSetCourse(Course.Top);
			}
			else if (KeyboardService.IsKeyDown(Keys.Right))
			{
				PlayerCharacterSetCourse(Course.Right);
			}
			else if (KeyboardService.IsKeyDown(Keys.Down))
			{
				PlayerCharacterSetCourse(Course.Bottom);
			}

			Update(Base, gameTime);

			PlayMaster.TakeStep();

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
			Base.Childs.Clear();

			var playground = PlayMaster.GetPlaygroundProxy();

			if (CreateTexture(playground.PlayerCharacter, "Character") is { } playerCharacter)
			{
				Base.Childs.Add(playerCharacter);
			}

			foreach (var barrier in playground.Obstructions)
			{
				if (CreateTexture(barrier, "Wall") is { } wall)
				{
					Base.Childs.Add(wall);
				}
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			Draw(Base, gameTime);

			SpriteBatch.End();
		}

		private Primitive? CreateTexture(IProxyBase<ICollided>? collided, string textureName)
		{
			if (collided?.Target is { Current: { } current })
			{
				var root = new Primitive(Base.ServiceProvider);
				var layout = new Layout()
				{
					Owner = Place,
					Margin = new()
					{
						Left = current.Border.Left,
						Top = current.Border.Top,
						Right = UnitKind.None,
						Bottom = UnitKind.None,
					},
					Size = new(current.Border.Width, current.Border.Height),
				};

				root.Childs.Add(new LayoutControl(Base.ServiceProvider)
				{
					Layout = new Layout()
					{
						Owner = layout.Place,
					},
				});

				root.Childs.Add(new TextureLayout(Base.ServiceProvider)
				{
					Name = textureName,
					Layout = new Layout()
					{
						Owner = layout.Place,
					},
				});

				return root;
			}
			else
			{
				return null;
			}
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